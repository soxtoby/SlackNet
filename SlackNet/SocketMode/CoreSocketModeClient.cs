using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocket4Net;

namespace SlackNet.SocketMode
{
    public interface ICoreSocketModeClient : IDisposable
    {
        Task Connect(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Is the client connecting or has it connected.
        /// </summary>
        bool Connected { get; }

        IObservable<string> RawSocketMessages { get; }
        IObservable<SocketMessage> Messages { get; }
        void Send(Acknowledgement acknowledgement);
    }

    public class CoreSocketModeClient : ICoreSocketModeClient
    {
        private readonly ISlackApiClient _client;
        private readonly IWebSocketFactory _webSocketFactory;
        private readonly SlackJsonSettings _jsonSettings;
        private readonly IScheduler _scheduler;
        private readonly Subject<string> _socketStringsSubject = new();
        private readonly ISubject<string> _rawSocketStrings;
        private IWebSocket _webSocket;
        private IDisposable _socketStringsSubscription;
        private IDisposable _reconnection;

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

            _rawSocketStrings = Subject.Synchronize(_socketStringsSubject);
            Messages = _rawSocketStrings
                .Select(m => JsonConvert.DeserializeObject<SocketMessage>(m, _jsonSettings.SerializerSettings))
                .Publish()
                .RefCount();
        }

        public async Task Connect(CancellationToken? cancellationToken = null)
        {
            var openResponse = await _client.AppsConnectionsApi.Open(cancellationToken).ConfigureAwait(false);

            _webSocket?.Dispose();
            _webSocket = _webSocketFactory.Create(openResponse.Url);

            var openedTask = _webSocket.Opened
                .Merge(_webSocket.Errors.SelectMany(Observable.Throw<Unit>))
                .FirstAsync()
                .ToTask(cancellationToken);

            _socketStringsSubscription = _webSocket.Messages
                .Subscribe(_rawSocketStrings);

            _reconnection?.Dispose();
            _reconnection = _webSocket.Closed
                .SelectMany(_ => Observable.FromAsync(() => Connect(cancellationToken), _scheduler)
                    .RetryWithDelay(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5), _scheduler))
                .TakeUntil(_rawSocketStrings.OfType<Disconnect>().Where(d => d.Reason == DisconnectReason.SocketModeDisabled))
                .Subscribe();

            _webSocket.Open();
            await openedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Is the client connecting or has it connected.
        /// </summary>
        public bool Connected =>
            _webSocket?.State == WebSocketState.Connecting
            || _webSocket?.State == WebSocketState.Open;

        public IObservable<string> RawSocketMessages => _rawSocketStrings.AsObservable();

        public IObservable<SocketMessage> Messages { get; }

        public void Send(Acknowledgement acknowledgement) =>
            _webSocket.Send(JsonConvert.SerializeObject(acknowledgement, _jsonSettings.SerializerSettings));

        public void Dispose()
        {
            _socketStringsSubject?.Dispose();
            _socketStringsSubscription?.Dispose();
            _webSocket?.Dispose();
            _reconnection?.Dispose();
        }
    }
}