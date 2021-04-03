using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.SocketMode;
using WebSocket4Net;

namespace SlackNet
{
    class ReconnectingWebSocket : IDisposable
    {
        private readonly IWebSocketFactory _webSocketFactory;
        private readonly IScheduler _scheduler;
        private readonly ILogger _log;
        private readonly int _id;
        private readonly Subject<RawSocketMessage> _messagesSubject = new();
        private readonly ISubject<RawSocketMessage> _messages;
        private readonly Subject<Unit> _disposed = new();
        private IWebSocket _webSocket;
        private IDisposable _messagesSubscription;
        private IDisposable _reconnection;

        public ReconnectingWebSocket(IWebSocketFactory webSocketFactory, IScheduler scheduler, ILogger logger, int id)
        {
            _webSocketFactory = webSocketFactory;
            _scheduler = scheduler;
            _log = logger.ForSource<ReconnectingWebSocket>();
            _id = id;

            _messages = Subject.Synchronize(_messagesSubject);
        }

        public async Task Connect(Func<Task<string>> getWebSocketUrl, CancellationToken? cancellationToken = null)
        {
            var url = await getWebSocketUrl().ConfigureAwait(false);

            _webSocket?.Dispose();
            _webSocket = _webSocketFactory.Create(url);

            var openedTask = _webSocket.Opened
                .Merge(_webSocket.Errors.SelectMany(Observable.Throw<Unit>))
                .FirstAsync()
                .ToTask(cancellationToken);

            _messagesSubscription?.Dispose();
            _messagesSubscription = _webSocket.Messages
                .Select(m => new RawSocketMessage { SocketId = _id, RequestId = Guid.NewGuid().ToString("N"), Message = m })
                .Do(m => _log
                    .WithContext("RequestId", m.RequestId)
                    .WithContext("Message", m.Message)
                    .Data("Message received on socket {SocketId}", _id))
                .Subscribe(_messages);

            _webSocket.Open();
            await openedTask.ConfigureAwait(false);

            _log.WithContext("Url", url)
                .Internal("Socket {SocketId} opened", _id);

            _reconnection?.Dispose();
            _reconnection = _webSocket.Closed
                .Do(_ => _log.Internal("Socket {SocketId} closed", _id))
                .SelectMany(_ => Observable.FromAsync(() => Connect(getWebSocketUrl, cancellationToken), _scheduler)
                    .RetryWithDelay(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5), _scheduler,
                        (e, d) => _log.Internal(e, "Error connecting socket {SocketId} - retrying in {RetryDelay}", _id, d)))
                .TakeUntil(_disposed)
                .Subscribe();
        }

        public WebSocketState State => _webSocket?.State ?? WebSocketState.None;

        public IObservable<RawSocketMessage> Messages => _messages.AsObservable();

        public void Send(string message) => _webSocket.Send(message);

        public void Dispose()
        {
            _disposed.OnNext(Unit.Default);
            _disposed.Dispose();
            _webSocket?.Dispose();
            _messagesSubject?.Dispose();
            _messagesSubscription?.Dispose();
            _reconnection?.Dispose();
        }
    }
}