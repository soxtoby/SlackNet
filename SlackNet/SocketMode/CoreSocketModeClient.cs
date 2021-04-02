using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocket4Net;

namespace SlackNet.SocketMode
{
    public interface ICoreSocketModeClient : IDisposable
    {
        Task Connect(CancellationToken? cancellationToken = null);
        void Disconnect();

        /// <summary>
        /// Is the client connecting or has it connected.
        /// </summary>
        bool Connected { get; }

        IObservable<RawSocketMessage> RawSocketMessages { get; }

        IObservable<SocketMessage> Messages { get; }

        /// <summary>
        /// Sends an acknowledgement response, with an optional payload, back to Slack.
        /// </summary>
        /// <param name="socketId">
        /// The ID of the web socket to send the acknowledgement on.
        /// Must be the same as the web socket that received the message being acknowledged.
        /// </param>
        /// <param name="acknowledgement">
        /// The response to send to Slack.
        /// Should contain the <see cref="SocketEnvelope.EnvelopeId"/> of the message being responded to.
        /// </param>
        void Send(int socketId, Acknowledgement acknowledgement);
    }

    public class CoreSocketModeClient : ICoreSocketModeClient
    {
        private readonly ISlackApiClient _client;
        private readonly IWebSocketFactory _webSocketFactory;
        private readonly SlackJsonSettings _jsonSettings;
        private readonly IScheduler _scheduler;
        private readonly Subject<RawSocketMessage> _rawSocketMessagesSubject = new();
        private readonly ISubject<RawSocketMessage> _rawSocketMessages;
        private List<ReconnectingWebSocket> _webSockets = new();
        private int _numConnections = 2;
        private TimeSpan _connectionDelay = TimeSpan.FromSeconds(10);
        private IDisposable _rawSocketStringsSubscription;
        private CancellationTokenSource _disconnectCancellation;
        private CancellationTokenSource _connectionCancelled;

        public CoreSocketModeClient(string appLevelToken)
            : this(
                new SlackApiClient(appLevelToken),
                Default.WebSocketFactory,
                Default.JsonSettings(),
                Default.Scheduler
            ) { }

        public CoreSocketModeClient(
            ISlackApiClient client,
            IWebSocketFactory webSocketFactory,
            SlackJsonSettings jsonSettings,
            IScheduler scheduler)
        {
            _client = client;
            _webSocketFactory = webSocketFactory;
            _jsonSettings = jsonSettings;
            _scheduler = scheduler;

            _rawSocketMessages = Subject.Synchronize(_rawSocketMessagesSubject);

            Messages = _rawSocketMessages
                .Select(DeserializeMessage)
                .Publish()
                .RefCount();

            Messages
                .OfType<Disconnect>()
                .Where(d => d.Reason == DisconnectReason.SocketModeDisabled)
                .Subscribe(_ => Disconnect());
        }

        private SocketMessage DeserializeMessage(RawSocketMessage rawMessage)
        {
            var message = JsonConvert.DeserializeObject<SocketMessage>(rawMessage.Message, _jsonSettings.SerializerSettings);
            message.SocketId = rawMessage.SocketId;
            return message;
        }

        /// <summary>
        /// Number of connections to create.
        /// If the client is already connected, changing this has no effect.
        /// </summary>
        public int NumConnections
        {
            get => _numConnections;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(NumConnections), "Must have at least 1 connection");
                _numConnections = value;
            }
        }

        /// <summary>
        /// Delay between creating connections, to avoid connections expiring at the same time.
        /// If the client is already connected, changing this has no effect.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan ConnectionDelay
        {
            get => _connectionDelay;
            set
            {
                if (value < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(ConnectionDelay), "Delay cannot be negative");
                _connectionDelay = value;
            }
        }

        public async Task Connect(CancellationToken? cancellationToken = null)
        {
            if (Connected)
                throw new InvalidOperationException("Already connecting or connected");

            _rawSocketStringsSubscription?.Dispose();
            Disconnect();

            _disconnectCancellation = new CancellationTokenSource();
            _connectionCancelled = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken ?? CancellationToken.None, _disconnectCancellation.Token);

            _webSockets = Enumerable.Range(0, NumConnections)
                .Select(_ => new ReconnectingWebSocket(_webSocketFactory, _scheduler))
                .ToList();

            _rawSocketStringsSubscription = _webSockets
                .Select((ws, i) => ws.Messages.Select(m => new RawSocketMessage { SocketId = i, Message = m }))
                .Merge()
                .Subscribe(_rawSocketMessages);

            var firstConnection = _webSockets.First().Connect(GetWebSocketUrl, _connectionCancelled.Token);

            // Stagger remaining connections so they don't all expire at the same time
            _webSockets.Skip(1).ToObservable()
                .Zip(Observable.Interval(ConnectionDelay, _scheduler).Take(_webSockets.Count - 1), (ws, _) => ws)
                .Select(ws => ws.Connect(GetWebSocketUrl, _connectionCancelled.Token))
                .Subscribe();

            await firstConnection.ConfigureAwait(false);

            async Task<string> GetWebSocketUrl()
            {
                var openResponse = await _client.AppsConnectionsApi.Open(_connectionCancelled.Token).ConfigureAwait(false);
                return openResponse.Url;
            }
        }

        public void Disconnect()
        {
            _disconnectCancellation?.Cancel();
            foreach (var webSocket in _webSockets)
                webSocket.Dispose();
        }

        /// <summary>
        /// Is the client connecting or has it connected.
        /// </summary>
        public bool Connected =>
            _webSockets.Any(ws =>
                ws.State == WebSocketState.Connecting
                || ws.State == WebSocketState.Open);

        public IObservable<RawSocketMessage> RawSocketMessages => _rawSocketMessages.AsObservable();

        public IObservable<SocketMessage> Messages { get; }

        /// <summary>
        /// Sends an acknowledgement response, with an optional payload, back to Slack.
        /// </summary>
        /// <param name="socketId">
        /// The ID of the web socket to send the acknowledgement on.
        /// Must be the same as the web socket that received the message being acknowledged.
        /// </param>
        /// <param name="acknowledgement">
        /// The response to send to Slack.
        /// Should contain the <see cref="SocketEnvelope.EnvelopeId"/> of the message being responded to.
        /// </param>
        public void Send(int socketId, Acknowledgement acknowledgement) =>
            _webSockets.ElementAtOrDefault(socketId)?.Send(JsonConvert.SerializeObject(acknowledgement, _jsonSettings.SerializerSettings));

        public void Dispose()
        {
            _connectionCancelled?.Dispose();
            _disconnectCancellation?.Dispose();
            _rawSocketStringsSubscription.Dispose();
            _rawSocketMessagesSubject.Dispose();
            foreach (var webSocket in _webSockets)
                webSocket.Dispose();
        }
    }
}