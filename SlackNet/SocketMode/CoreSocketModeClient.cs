#nullable enable
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet.SocketMode;

public interface ICoreSocketModeClient : IDisposable, IAsyncDisposable
{
    Task Connect(SocketModeConnectionOptions? connectionOptions = null, CancellationToken cancellationToken = default);

    [Obsolete("Use DisconnectAsync instead.")]
    void Disconnect();

    /// <summary>
    /// Disconnects and waits for all socket connection work to stop.
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Is the client connecting or has it connected?
    /// </summary>
    bool Connected { get; }

    IObservable<RawSocketMessage> RawSocketMessages { get; }

    IObservable<SocketMessage> Messages { get; }

    /// <summary>
    /// Sends an acknowledgement response, with an optional payload, back to Slack.
    /// </summary>
    /// <param name="socketId">
    ///     The ID of the web socket to send the acknowledgement on.
    ///     Must be the same as the web socket that received the message being acknowledged.
    /// </param>
    /// <param name="acknowledgement">
    ///     The response to send to Slack.
    ///     Should contain the <see cref="SocketEnvelope.EnvelopeId"/> of the message being responded to.
    /// </param>
    Task Send(int socketId, Acknowledgement acknowledgement);
}

public class CoreSocketModeClient : ICoreSocketModeClient
{
    private readonly ISlackApiClient _client;
    private readonly IWebSocketFactory _webSocketFactory;
    private readonly SlackJsonSettings _jsonSettings;
    private readonly IScheduler _scheduler;
    private readonly ILogger _log;
    private readonly Lock _connectionLock = new();
    private readonly Subject<RawSocketMessage> _rawSocketMessagesSubject = new();
    private readonly ISubject<RawSocketMessage> _rawSocketMessages;
    private SocketConnections? _connections;
    private Task _disconnectTask = Task.CompletedTask;

    public CoreSocketModeClient(string appLevelToken)
        : this(
            new SlackApiClient(appLevelToken),
            Default.WebSocketFactory,
            Default.JsonSettings(),
            Default.Scheduler,
            Default.Logger
        ) { }

    public CoreSocketModeClient(
        ISlackApiClient client,
        IWebSocketFactory webSocketFactory,
        SlackJsonSettings jsonSettings,
        IScheduler scheduler,
        ILogger logger)
    {
        _client = client;
        _webSocketFactory = webSocketFactory;
        _jsonSettings = jsonSettings;
        _scheduler = scheduler;
        _log = logger.ForSource<CoreSocketModeClient>();

        _rawSocketMessages = Subject.Synchronize(_rawSocketMessagesSubject);

        Messages = _rawSocketMessages
            .Select(DeserializeMessage)
            .WhereNotNull()
            .Publish()
            .RefCount();

        Messages
            .OfType<Disconnect>()
            .Do(m => _log.Internal("Socket {SocketId} disconnecting because {Reason}", m.SocketId, m.Reason))
            .Where(d => d.Reason == DisconnectReason.SocketModeDisabled)
            .SelectMany(_ => Observable.FromAsync(DisconnectAsync))
            .Subscribe();
    }

    private SocketMessage? DeserializeMessage(RawSocketMessage rawMessage)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<SocketMessage>(rawMessage.Message, _jsonSettings.SerializerSettings)!;
            message.SocketId = rawMessage.SocketId;
            message.RequestId = rawMessage.RequestId;
            return message;
        }
        catch (Exception e)
        {
            _log.WithContext("SocketId", rawMessage.SocketId)
                .WithContext("RequestId", rawMessage.RequestId)
                .WithContext("Message", rawMessage.Message)
                .Error(e, "Error deserializing socket mode message");
            return null;
        }
    }

    public async Task Connect(SocketModeConnectionOptions? connectionOptions = null, CancellationToken cancellationToken = default)
    {
        lock (_connectionLock)
        {
            if (Connected)
                throw new InvalidOperationException("Already connecting or connected");
        }

        connectionOptions ??= Default.SocketModeConnectionOptions;

        _log.Internal("Opening {NumberOfConnections} socket mode connections, with delay of {ConnectionDelay}", connectionOptions.NumberOfConnections, connectionOptions.ConnectionDelay);

        await DisconnectAsync().ConfigureAwait(false);

        Task firstConnection;
        lock (_connectionLock)
        {
            if (_connections is not null)
                throw new InvalidOperationException("Already connecting or connected");

            var disconnectCancellation = new CancellationTokenSource();
            var connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disconnectCancellation.Token);

            var webSockets = Enumerable.Range(0, connectionOptions.NumberOfConnections)
                .Select(i => new ReconnectingWebSocket(_webSocketFactory, _scheduler, _log, i))
                .ToArray();

            var rawSocketStringsSubscription = webSockets
                .Select(ws => ws.Messages)
                .Merge()
                .Subscribe(_rawSocketMessages);

            firstConnection = webSockets.First().Connect(GetWebSocketUrl, connectionCancellation.Token);

            // Stagger remaining connections so they don't all expire at the same time
            var additionalConnectionsTask = Task.WhenAll(webSockets.Skip(1).Select(ConnectAdditional));
            _connections = new SocketConnections(
                disconnectCancellation,
                connectionCancellation,
                webSockets,
                additionalConnectionsTask,
                rawSocketStringsSubscription);

            async Task<string> GetWebSocketUrl()
            {
                var openResponse = await _client.AppsConnectionsApi.Open(connectionCancellation.Token).ConfigureAwait(false);
                return connectionOptions.DebugReconnects
                    ? openResponse.Url + "&debug_reconnects=true"
                    : openResponse.Url;
            }

            async Task ConnectAdditional(ReconnectingWebSocket webSocket, int index)
            {
                try
                {
                    await Observable.Interval(connectionOptions.ConnectionDelay, _scheduler)
                        .ElementAt(index)
                        .ToTask(connectionCancellation.Token)
                        .ConfigureAwait(false);
                    await webSocket.Connect(GetWebSocketUrl, connectionCancellation.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (connectionCancellation.IsCancellationRequested)
                {
                    // Disconnecting.
                }
                catch (Exception e)
                {
                    _log.Error(e, "Error opening additional socket mode connection");
                }
            }
        }

        await firstConnection.ConfigureAwait(false);
    }

    [Obsolete("Use DisconnectAsync instead.")]
    public void Disconnect() => DisconnectAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Disconnects and waits for all socket connection work to stop.
    /// </summary>
    public Task DisconnectAsync()
    {
        lock (_connectionLock)
        {
            var connections = _connections;
            if (connections is null)
                return _disconnectTask;

            _connections = null;
            _disconnectTask = DisconnectCore(connections);
            return _disconnectTask;
        }
    }

    private async Task DisconnectCore(SocketConnections connections)
    {
        _log.Internal("Disconnecting previous socket mode connections");
        await connections.DisconnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Is the client connecting or has it connected?
    /// </summary>
    public bool Connected =>
        _connections?.WebSockets.Any(ws => ws.State is WebSocketState.Connecting or WebSocketState.Open) == true;

    public IObservable<RawSocketMessage> RawSocketMessages => _rawSocketMessages.AsObservable();

    public IObservable<SocketMessage> Messages { get; }

    /// <summary>
    /// Sends an acknowledgement response, with an optional payload, back to Slack.
    /// </summary>
    /// <param name="socketId">
    ///     The ID of the web socket to send the acknowledgement on.
    ///     Must be the same as the web socket that received the message being acknowledged.
    /// </param>
    /// <param name="acknowledgement">
    ///     The response to send to Slack.
    ///     Should contain the <see cref="SocketEnvelope.EnvelopeId"/> of the message being responded to.
    /// </param>
    public async Task Send(int socketId, Acknowledgement acknowledgement)
    {
        if (_connections?.WebSockets.ElementAtOrDefault(socketId) is ReconnectingWebSocket socket)
            await socket.Send(JsonConvert.SerializeObject(acknowledgement, _jsonSettings.SerializerSettings)).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync().ConfigureAwait(false);
        _rawSocketMessagesSubject.Dispose();
    }

    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();

    private sealed class SocketConnections(
        CancellationTokenSource disconnectCancellation,
        CancellationTokenSource connectionCancellation,
        ReconnectingWebSocket[] webSockets,
        Task additionalConnectionsTask,
        IDisposable rawSocketStringsSubscription)
    {
        public ReconnectingWebSocket[] WebSockets { get; } = webSockets;

        public async Task DisconnectAsync()
        {
            try
            {
                await disconnectCancellation.CancelAsync().ConfigureAwait(false);
                await Task.WhenAll(WebSockets
                    .Select(webSocket => webSocket.DisposeAsync().AsTask())
                    .Append(additionalConnectionsTask)).ConfigureAwait(false);
            }
            finally
            {
                rawSocketStringsSubscription.Dispose();
                connectionCancellation.Dispose();
                disconnectCancellation.Dispose();
            }
        }
    }
}
