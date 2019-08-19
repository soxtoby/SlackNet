using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet.Bot
{
    public interface ISlackBot : IObserver<BotMessage>
    {
        /// <summary>
        /// Id of the bot user.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Name of the bot user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Stream of new messages received from Slack.
        /// </summary>
        IObservable<IMessage> Messages { get; }

        /// <summary>
        /// Connect to Slack.
        /// </summary>
        Task Connect(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Transform stream of incoming messages.
        /// </summary>
        void AddIncomingMiddleware(Func<IObservable<IMessage>, IObservable<IMessage>> middleware);

        /// <summary>
        /// Transform stream of outgoing messages.
        /// </summary>
        void AddOutgoingMiddleware(Func<IObservable<BotMessage>, IObservable<BotMessage>> middleware);

        /// <summary>
        /// Add a handler object for handling new messages received from Slack.
        /// </summary>
        void AddHandler(IMessageHandler handler);

        /// <summary>
        /// Fired when a new message is received from Slack.
        /// </summary>
        event EventHandler<IMessage> OnMessage;

        /// <summary>
        /// Get information on a public or private channel, IM, or multi-person IM.
        /// </summary>
        Task<Hub> GetHubById(string hubId);

        /// <summary>
        /// Find hub with matching name.
        /// </summary>
        /// <param name="channel">Channel, group or IM name, with leading # or @ symbol as appropriate.</param>
        Task<Hub> GetHubByName(string channel);

        /// <summary>
        /// Find channel by name, with or without leading #.
        /// </summary>
        Task<Hub> GetChannelByName(string name);

        /// <summary>
        /// Find private group by name.
        /// </summary>
        Task<Hub> GetGroupByName(string name);

        /// <summary>
        /// Find user by name, with or without leading @.
        /// </summary>
        Task<Im> GetImByName(string username);

        /// <summary>
        /// Get and open Im by user ID.
        /// </summary>
        Task<Im> GetImByUserId(string userId);

        /// <summary>
        /// Get full list of public channels.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<Channel>> GetChannels();

        /// <summary>
        /// Get list of private groups that the bot is in.
        /// </summary>
        Task<IReadOnlyList<Channel>> GetGroups();

        /// <summary>
        /// Get list of multi-person IMs that the bot is in.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<Channel>> GetMpIms();

        /// <summary>
        /// Get list of IMs that have been opened with the bot.
        /// </summary>
        Task<IReadOnlyList<Im>> GetIms();

        /// <summary>
        /// Get user information.
        /// </summary>
        Task<User> GetUserById(string userId);

        /// <summary>
        /// Find user by username, with or without leading @.
        /// </summary>
        Task<User> GetUserByName(string username);

        /// <summary>
        /// Get full list of users.
        /// </summary>
        Task<IReadOnlyList<User>> GetUsers();

        /// <summary>
        /// Send a message to Slack as the bot.
        /// </summary>
        Task Send(BotMessage message);

        /// <summary>
        /// Show typing indicator in Slack while performing some action.
        /// </summary>
        Task WhileTyping(string channelId, Func<Task> action);

        /// <summary>
        /// Clear bot's cache of hubs, users etc.
        /// </summary>
        void ClearCache();
    }

    public class SlackBot : ISlackBot, IDisposable
    {
        private readonly ISlackRtmClient _rtm;
        private readonly ISlackApiClient _api;
        private readonly IScheduler _scheduler;
        private readonly ConcurrentQueue<IMessageHandler> _handlers = new ConcurrentQueue<IMessageHandler>();
        private readonly ConcurrentDictionary<string, Task<Hub>> _hubs = new ConcurrentDictionary<string, Task<Hub>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Channel>>> _channels = new ConcurrentValue<Task<IReadOnlyList<Channel>>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Channel>>> _groups = new ConcurrentValue<Task<IReadOnlyList<Channel>>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Channel>>> _mpims = new ConcurrentValue<Task<IReadOnlyList<Channel>>>();
        private readonly ConcurrentDictionary<string, Task<User>> _users = new ConcurrentDictionary<string, Task<User>>();
        private readonly ConcurrentDictionary<string, Task<BotInfo>> _bots = new ConcurrentDictionary<string, Task<BotInfo>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<User>>> _allUsers = new ConcurrentValue<Task<IReadOnlyList<User>>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Im>>> _ims = new ConcurrentValue<Task<IReadOnlyList<Im>>>();
        private readonly SyncedSubject<IMessage> _incomingMessages = new SyncedSubject<IMessage>();
        private readonly SyncedSubject<BotMessage> _outgoingMessages = new SyncedSubject<BotMessage>();
        private readonly SyncedSubject<BotMessage> _sentMessages = new SyncedSubject<BotMessage>();
        private IObservable<IMessage> _incomingWithMiddlewareApplied;
        private IObservable<BotMessage> _outgoingWithMiddlewareApplied;
        private IDisposable _outgoingSubscription;
        private IDisposable _incomingSubscription;

        public SlackBot(string token) : this(new SlackRtmClient(token), new SlackApiClient(token)) { }

        public SlackBot(ISlackRtmClient rtmClient, ISlackApiClient apiClient, IScheduler scheduler = null)
        {
            _rtm = rtmClient;
            _api = apiClient;
            _scheduler = scheduler ?? Scheduler.Default;
            _incomingWithMiddlewareApplied = _rtm.Messages
                .Where(m => m.GetType() == typeof(MessageEvent) || m.GetType() == typeof(SlackNet.Events.BotMessage))
                .Where(m => m.User != Id)
                .SelectMany(CreateSlackMessage);
            _outgoingWithMiddlewareApplied = _outgoingMessages
                .LimitFrequency(TimeSpan.FromSeconds(1), _scheduler);
        }

        /// <summary>
        /// Id of the bot user.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Name of the bot user.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Connect to Slack.
        /// </summary>
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

        /// <summary>
        /// Transform stream of incoming messages.
        /// </summary>
        public void AddIncomingMiddleware(Func<IObservable<IMessage>, IObservable<IMessage>> middleware)
        {
            if (_rtm.Connected)
                throw new InvalidOperationException("Can't add more middleware after bot is connected.");

            _incomingWithMiddlewareApplied = middleware(_incomingWithMiddlewareApplied);
        }

        /// <summary>
        /// Transform stream of outgoing messages.
        /// </summary>
        public void AddOutgoingMiddleware(Func<IObservable<BotMessage>, IObservable<BotMessage>> middleware)
        {
            if (_rtm.Connected)
                throw new InvalidOperationException("Can't add more middleware after bot is connected.");

            _outgoingWithMiddlewareApplied = middleware(_outgoingWithMiddlewareApplied);
        }

        /// <summary>
        /// Add a handler object for handling new messages received from Slack.
        /// </summary>
        public void AddHandler(IMessageHandler handler) => _handlers.Enqueue(handler);
        /// <summary>
        /// Fired when a new message is received from Slack.
        /// </summary>
        public event EventHandler<IMessage> OnMessage;
        /// <summary>
        /// Stream of new messages received from Slack.
        /// </summary>
        public IObservable<IMessage> Messages => _incomingMessages.AsObservable();

        private async Task<SlackMessage> CreateSlackMessage(MessageEvent message)
        {
            SlackMessage result;

            if (message.User == null && (message is SlackNet.Events.BotMessage b))
            {
                BotInfo botInfo = await GetBotUserIdById(b.BotId);
                message.User = botInfo.UserId;
            }

            result = new SlackMessage(this)
            {
                Ts = message.Ts,
                ThreadTs = message.ThreadTs,
                Text = message.Text,
                Hub = await GetHubById(message.Channel).ConfigureAwait(false),
                User = await GetUserById(message.User).ConfigureAwait(false),
                Attachments = message.Attachments
            };

            return result;
        }

        private void HandleMessage(IMessage message)
        {
            OnMessage?.Invoke(this, message);
            _handlers
                .ToList()
                .ForEach(h => h.HandleMessage(message));
            _incomingMessages.OnNext(message);
        }

        /// <summary>
        /// Get information on a public or private channel, IM, or multi-person IM.
        /// </summary>
        public async Task<Hub> GetHubById(string hubId) =>
            String.IsNullOrEmpty(hubId)
                ? null
                : await _hubs.GetOrAdd(hubId, FetchHub).ConfigureAwait(false);

        private async Task<Hub> FetchHub(string hubId) =>
              hubId[0] == 'C' ? await _api.Channels.Info(hubId).NullIfNotFound().ConfigureAwait(false)
            : hubId[0] == 'G' ? await _api.Groups.Info(hubId).NullIfNotFound().ConfigureAwait(false)
            : hubId[0] == 'D' ? (Hub)(await _api.Conversations.OpenAndReturnInfo(hubId).NullIfNotFound().ConfigureAwait(false))?.Channel
            : null;

        /// <summary>
        /// Find hub with matching name.
        /// </summary>
        /// <param name="channel">Channel, group or IM name, with leading # or @ symbol as appropriate.</param>
        public async Task<Hub> GetHubByName(string channel) =>
              channel.FirstOrDefault() == '#' ? await GetChannelByName(channel).ConfigureAwait(false)
            : channel.FirstOrDefault() == '@' ? await GetImByName(channel).ConfigureAwait(false)
            : await GetGroupByName(channel).ConfigureAwait(false);

        /// <summary>
        /// Find channel by name, with or without leading #.
        /// </summary>
        public async Task<Hub> GetChannelByName(string name) =>
            await FindCachedHub<Channel>(h => h.IsChannel && h.Name == WithoutLeadingHash(name)).ConfigureAwait(false)
            ?? (await GetChannels().ConfigureAwait(false))
                .FirstOrDefault(c => c.Name == WithoutLeadingHash(name));

        private static string WithoutLeadingHash(string name) => name.TrimStart('#');

        /// <summary>
        /// Find private group by name.
        /// </summary>
        public async Task<Hub> GetGroupByName(string name) =>
            await FindCachedHub<Channel>(h => h.IsGroup && h.Name == name).ConfigureAwait(false)
            ?? (await GetGroups().ConfigureAwait(false))
                .FirstOrDefault(g => g.Name == name);

        /// <summary>
        /// Get an open Im by user name, with or without leading @.
        /// </summary>
        public async Task<Im> GetImByName(string username) =>
            await GetImByUserId(
                (await GetUserByName(username).ConfigureAwait(false))?.Id).ConfigureAwait(false);

        /// <summary>
        /// Get and open Im by user ID.
        /// </summary>
        public async Task<Im> GetImByUserId(string userId) =>
            userId == null ? null
                : await FindCachedHub<Im>(h => h.User == userId).ConfigureAwait(false)
                    ?? await OpenAndCacheIm(userId).ConfigureAwait(false);

        private async Task<Im> OpenAndCacheIm(string userId)
        {
            var im = await OpenIm(userId).ConfigureAwait(false);
            if (im != null)
                _hubs[im.Id] = Task.FromResult((Hub)im);
            return im;
        }

        private Task<T> FindCachedHub<T>(Func<T, bool> predicate) where T : Hub => _hubs.Values.FirstOrDefaultAsync(predicate);

        private async Task<Im> OpenIm(string userId) => (await _api.Im.Open(userId, true).NullIfNotFound().ConfigureAwait(false))?.Channel;

        /// <summary>
        /// Get full list of public channels.
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyList<Channel>> GetChannels() => _channels.GetOrCreateValue(FetchChannels);
        private async Task<IReadOnlyList<Channel>> FetchChannels() => CacheHubs(await _api.Channels.List().ConfigureAwait(false));

        /// <summary>
        /// Get list of private groups that the bot is in.
        /// </summary>
        public Task<IReadOnlyList<Channel>> GetGroups() => _groups.GetOrCreateValue(FetchGroups);
        private async Task<IReadOnlyList<Channel>> FetchGroups() => CacheHubs(await _api.Groups.List().ConfigureAwait(false));

        /// <summary>
        /// Get list of multi-person IMs that the bot is in.
        /// </summary>
        /// <returns></returns>
        public Task<IReadOnlyList<Channel>> GetMpIms() => _mpims.GetOrCreateValue(FetchMpims);
        private async Task<IReadOnlyList<Channel>> FetchMpims() => CacheHubs(await _api.Mpim.List().ConfigureAwait(false));

        private IReadOnlyList<Channel> CacheHubs(IReadOnlyList<Channel> channels)
        {
            foreach (var channel in channels)
                _hubs[channel.Id] = Task.FromResult((Hub)channel);
            return channels;
        }

        /// <summary>
        /// Get list of IMs that have been opened with the bot.
        /// </summary>
        public async Task<IReadOnlyList<Im>> GetIms() => await _ims.GetOrCreateValue(() => _api.Im.List()).ConfigureAwait(false);

        /// <summary>
        /// Get user information.
        /// </summary>
        public async Task<User> GetUserById(string userId) =>
            String.IsNullOrEmpty(userId)
                ? null
                : await _users.GetOrAdd(userId, _ => _api.Users.Info(userId).NullIfNotFound()).ConfigureAwait(false);

        /// <summary>
        /// Get user information.
        /// </summary>
        public async Task<BotInfo> GetBotUserIdById(string botId) =>
            String.IsNullOrEmpty(botId)
                ? null
                : await _bots.GetOrAdd(botId, _ => _api.Bots.Info(botId).NullIfNotFound()).ConfigureAwait(false);

        /// <summary>
        /// Find user by username, with or without leading @.
        /// </summary>
        public async Task<User> GetUserByName(string username) =>
            await _users.Values.FirstOrDefaultAsync(u => u.Name == WithoutLeadingAt(username)).ConfigureAwait(false)
            ?? (await GetUsers().ConfigureAwait(false)).FirstOrDefault(u => u.Name == WithoutLeadingAt(username));

        private static string WithoutLeadingAt(string name) => name.TrimStart('@');

        /// <summary>
        /// Get full list of users.
        /// </summary>
        public Task<IReadOnlyList<User>> GetUsers() => _allUsers.GetOrCreateValue(FetchUsers);

        private async Task<IReadOnlyList<User>> FetchUsers()
        {
            var users = new List<User>();
            string cursor = null;
            do
            {
                var response = await _api.Users.List(cursor).ConfigureAwait(false);

                users.AddRange(response.Members);
                foreach (var user in response.Members)
                    _users[user.Id] = Task.FromResult(user);

                cursor = response.ResponseMetadata?.NextCursor;
            } while (!string.IsNullOrEmpty(cursor));
            return users;
        }

        /// <summary>
        /// Send a message to Slack as the bot.
        /// </summary>
        public async Task Send(BotMessage message)
        {
            var sent = _sentMessages.FirstOrDefaultAsync(m => m == message).ToTask();
            _outgoingMessages.OnNext(message);
            await sent.ConfigureAwait(false);
        }

        private async Task<PostMessageResponse> PostMessage(BotMessage message)
        {
            var response = await _api.Chat.PostMessage(new Message
            {
                Channel = message.Hub != null
                        ? await message.Hub.HubId(this).ConfigureAwait(false)
                        : message.ReplyTo?.Hub?.Id,
                Text = message.Text,
                Attachments = message.Attachments,
                Blocks = message.Blocks,
                ThreadTs = await ReplyingInDifferentHub(message).ConfigureAwait(false)
                        ? null
                        : message.ReplyTo?.ThreadTs
                        ?? (message.CreateThread ? message.ReplyTo?.Ts : null),
                ReplyBroadcast = message.ReplyBroadcast,
                Parse = message.Parse,
                LinkNames = message.LinkNames,
                UnfurlLinks = message.UnfurlLinks,
                UnfurlMedia = message.UnfurlMedia,
                AsUser = true
            }).ConfigureAwait(false);
            _sentMessages.OnNext(message);
            return response;
        }

        private async Task<bool> ReplyingInDifferentHub(BotMessage message)
        {
            return message.Hub != null && await message.Hub.HubId(this).ConfigureAwait(false) != message.ReplyTo?.Hub.Id;
        }

        /// <summary>
        /// Show typing indicator in Slack while performing some action.
        /// </summary>
        public async Task WhileTyping(string channelId, Func<Task> action)
        {
            using (Observable.Interval(TimeSpan.FromSeconds(4), _scheduler).Subscribe(_ => _rtm.SendTyping(channelId)))
                await action().ConfigureAwait(false);
        }

        /// <summary>
        /// Clear bot's cache of hubs, users etc.
        /// </summary>
        public void ClearCache()
        {
            _hubs.Clear();
            _channels.Clear();
            _groups.Clear();
            _mpims.Clear();
            _users.Clear();
            _allUsers.Clear();
            _ims.Clear();
        }

        public void OnCompleted() => _outgoingMessages.OnCompleted();
        public void OnError(Exception error) => _outgoingMessages.OnError(error);
        public void OnNext(BotMessage value) => _outgoingMessages.OnNext(value);

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
