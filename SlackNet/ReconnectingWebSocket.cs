using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.SocketMode;
using WebSocket4Net;

namespace SlackNet;

class ReconnectingWebSocket : IDisposable
{
    private readonly IWebSocketFactory _webSocketFactory;
    private readonly IScheduler _scheduler;
    private readonly ILogger _log;
    private readonly int _id;
    private readonly Subject<RawSocketMessage> _messagesSubject = new();
    private readonly ISubject<RawSocketMessage> _messages;
    private readonly CancellationTokenSource _disposed = new();
    private IWebSocket _webSocket;
    private IDisposable _messagesSubscription;

    public ReconnectingWebSocket(IWebSocketFactory webSocketFactory, IScheduler scheduler, ILogger logger, int id)
    {
        _webSocketFactory = webSocketFactory;
        _scheduler = scheduler;
        _log = logger.ForSource<ReconnectingWebSocket>();
        _id = id;

        _messages = Subject.Synchronize(_messagesSubject);
    }

    public async Task Connect(Func<Task<string>> getWebSocketUrl, CancellationToken cancellationToken = default)
    {
        using var cancel = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposed.Token);

        // Retry as long as not cancelled and Slack doesn't return an error response
        await Observable.FromAsync(() => ConnectInternal(getWebSocketUrl, cancel.Token), _scheduler)
            .RetryWithDelay(e => e is not SlackException and not TaskCanceledException, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5), _scheduler,
                (e, d) => _log.Internal(e, "Error connecting socket {SocketId} - retrying in {RetryDelay}", _id, d))
            .FirstAsync()
            .ToTask(cancel.Token)
            .ConfigureAwait(false);
    }

    private async Task ConnectInternal(Func<Task<string>> getWebSocketUrl, CancellationToken cancellationToken)
    {
        var url = await getWebSocketUrl().ConfigureAwait(false);

        _webSocket?.Dispose();
        _webSocket = _webSocketFactory.Create(url);

        _messagesSubscription?.Dispose();
        _messagesSubscription = _webSocket.Messages
            .Select(m => new RawSocketMessage { SocketId = _id, RequestId = Guid.NewGuid().ToString("N"), Message = m })
            .Do(m => _log
                .WithContext("RequestId", m.RequestId)
                .WithContext("Message", m.Message)
                .Data("Message received on socket {SocketId}", _id))
            .Subscribe(_messages);

        _log.Internal("Opening socket {SocketId}", _id);
        if (!await _webSocket.Open(cancellationToken).ConfigureAwait(false))
            throw new ConnectionFailedException(_id, State);

        _log.WithContext("Url", url)
            .Internal("Socket {SocketId} opened", _id);

        _ = ReconnectOnClose(getWebSocketUrl, cancellationToken);
    }

    private async Task ReconnectOnClose(Func<Task<string>> getWebSocketUrl, CancellationToken cancellationToken)
    {
        await _webSocket.Closed.ConfigureAwait(false);
        
        _log.Internal("Socket {SocketId} closed", _id);
        
        if (!cancellationToken.IsCancellationRequested)
            await Connect(getWebSocketUrl, cancellationToken).ConfigureAwait(false);
    }

    public WebSocketState State => _webSocket?.State ?? WebSocketState.None;

    public IObservable<RawSocketMessage> Messages => _messages.AsObservable();

    public void Send(string message) => _webSocket.Send(message);

    public void Dispose()
    {
        _disposed.Cancel();
        _disposed.Dispose();
        _messagesSubscription?.Dispose();
        _messagesSubject?.Dispose();
        _webSocket?.Dispose();
    }
}

class ConnectionFailedException(int socketId, WebSocketState state) : Exception($"Failed to open socket {socketId}; socket in {state} state.");