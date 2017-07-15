using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet;
using SlackNet.WebApi;

namespace SlackNet.Bot
{
    public class SlackBot : IObservable<IMessage>, IObserver<Message>, IDisposable
    {
        private readonly SlackRtmClient _rtm;
        private readonly SlackApiClient _api;
        private readonly ConcurrentQueue<IMessageHandler> _handlers = new ConcurrentQueue<IMessageHandler>();
        private readonly ConcurrentDictionary<string, Task<Hub>> _hubs = new ConcurrentDictionary<string, Task<Hub>>();
        private readonly ConcurrentDictionary<string, Task<User>> _users = new ConcurrentDictionary<string, Task<User>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Im>>> _ims = new ConcurrentValue<Task<IReadOnlyList<Im>>>();
        private readonly SyncedSubject<IMessage> _incomingMessages = new SyncedSubject<IMessage>();
        private readonly SyncedSubject<Message> _outgoingMessages = new SyncedSubject<Message>();
        private readonly SyncedSubject<Message> _sentMessages = new SyncedSubject<Message>();
        private IObservable<IMessage> _incomingWithMiddlewareApplied;
        private IObservable<Message> _outgoingWithMiddlewareApplied;
        private IDisposable _outgoingSubscription;
        private IDisposable _incomingSubscription;

        public SlackBot(string token) : this(new SlackRtmClient(token), new SlackApiClient(token)) { }

        public SlackBot(SlackRtmClient rtmClient, SlackApiClient apiClient)
        {
            _rtm = rtmClient;
            _api = apiClient;
            _incomingWithMiddlewareApplied = _rtm.Messages.SelectMany(CreateBotMessage);
            _outgoingWithMiddlewareApplied = _outgoingMessages;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }

        public async Task Connect(CancellationToken? cancellationToken = null)
        {
            // If already connected, client will throw
            var connection = _rtm.Connect(cancellationToken: cancellationToken);

            _incomingSubscription = _incomingWithMiddlewareApplied
                .Subscribe(HandleMessage);
            _outgoingSubscription = _outgoingWithMiddlewareApplied
                .SelectMany(PostMessage)
                .Retry()
                .Subscribe();

            var connectResponse = await connection.ConfigureAwait(false);
            Id = connectResponse.Self.Id;
            Name = connectResponse.Self.Name;
        }

        public void AddIncomingMiddleware(Func<IObservable<IMessage>, IObservable<IMessage>> middleware)
        {
            if (_rtm.Connected)
                throw new InvalidOperationException("Can't add more middleware after bot is connected.");

            _incomingWithMiddlewareApplied = middleware(_incomingWithMiddlewareApplied);
        }

        public void AddOutgoingMiddleware(Func<IObservable<Message>, IObservable<Message>> middleware)
        {
            if (_rtm.Connected)
                throw new InvalidOperationException("Can't add more middleware after bot is connected.");

            _outgoingWithMiddlewareApplied = middleware(_outgoingWithMiddlewareApplied);
        }

        public void AddHandler(IMessageHandler handler) => _handlers.Enqueue(handler);
        public event EventHandler<IMessage> OnMessage;
        public IObservable<IMessage> Messages => _incomingMessages.AsObservable();

        private async Task<SlackMessage> CreateBotMessage(MessageEvent message)
        {
            return new SlackMessage(message, this)
                {
                    Ts = message.Ts,
                    Text = message.Text,
                    Hub = await GetHub(message.Channel).ConfigureAwait(false),
                    User = await GetUser(message.User).ConfigureAwait(false),
                    Attachments = message.Attachments
                };
        }

        private void HandleMessage(IMessage message)
        {
            OnMessage?.Invoke(this, message);
            _handlers
                .ToList()
                .ForEach(h => h.HandleMessage(message));
            _incomingMessages.OnNext(message);
        }

        private async Task<Hub> GetHub(string channelId)
        {
            return await _hubs.GetOrAdd(channelId, FetchHub).ConfigureAwait(false);
        }

        private async Task<Hub> FetchHub(string channelId)
        {
            switch (channelId[0])
            {
                case 'C':
                    return await _api.Channels.Info(channelId).NullIfNotFound().ConfigureAwait(false);
                case 'G':
                    return await _api.Groups.Info(channelId).NullIfNotFound().ConfigureAwait(false);
                case 'D':
                    return await FetchImChannel(channelId).ConfigureAwait(false);
            }
            return null;
        }

        private async Task<Hub> FetchImChannel(string channelId)
        {
            IReadOnlyList<Im> ims = await GetOpenIms().ConfigureAwait(false);
            Im matchingIm = ims.FirstOrDefault(im => im.Id == channelId);
            if (matchingIm == null)
                return null;

            ImResponse fullImResponse = await _api.Im.Open(matchingIm.User, true).NullIfNotFound().ConfigureAwait(false);
            return fullImResponse?.Channel;
        }

        public async Task<IReadOnlyList<Im>> GetOpenIms() => await _ims.GetOrCreateValue(() => _api.Im.List()).ConfigureAwait(false);

        public Task<User> GetUser(string userId) => _users.GetOrAdd(userId, _ => _api.Users.Info(userId).NullIfNotFound());

        public Task Send(Message message)
        {
            var sent = _sentMessages.FirstOrDefaultAsync(m => m == message).ToTask();
            _outgoingMessages.OnNext(message);
            return sent;
        }

        private Task<PostMessageResponse> PostMessage(Message message) => _api.Chat.PostMessage(message);

        public async Task WhileTyping(string channelId, Func<Task> action)
        {
            using (Observable.Interval(TimeSpan.FromSeconds(4)).Subscribe(_ => _rtm.SendTyping(channelId)))
                await action().ConfigureAwait(false);
        }

        public IDisposable Subscribe(IObserver<IMessage> observer) => _incomingMessages.Subscribe(observer);

        public void OnCompleted() => _outgoingMessages.OnCompleted();
        public void OnError(Exception error) => _outgoingMessages.OnError(error);
        public void OnNext(Message value) => _outgoingMessages.OnNext(value);

        public void Dispose()
        {
            _rtm.Dispose();
            _incomingMessages.Dispose();
            _outgoingMessages.Dispose();
            _incomingSubscription?.Dispose();
            _outgoingSubscription?.Dispose();
        }
    }
}
