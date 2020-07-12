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
        /// Get full list conversations the bot user has access to.
        /// </summary>
        Task<IReadOnlyCollection<Conversation>> GetConversations();

        /// <summary>
        /// Retrieve information about a conversation.
        /// </summary>
        Task<Conversation> GetConversationById(string conversationId);

        /// <summary>
        /// Find conversation with matching name.
        /// </summary>
        /// <param name="conversationName">Channel, group or IM name, with leading # or @ symbol as appropriate.</param>
        Task<Conversation> GetConversationByName(string conversationName);

        /// <summary>
        /// Get and open Im by user ID.
        /// </summary>
        Task<Conversation> GetConversationByUserId(string userId);

        /// <summary>
        /// Get user information.
        /// </summary>
        Task<User> GetUserById(string userId);

        /// <summary>
        /// Get bot user information.
        /// </summary>
        Task<BotInfo> GetBotUserById(string botId);

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
        Task Send(BotMessage message, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Show typing indicator in Slack while performing some action.
        /// </summary>
        Task WhileTyping(string channelId, Func<Task> action);

        /// <summary>
        /// Clear bot's cache of hubs, users etc.
        /// </summary>
        void ClearCache();
        
        #region Hubs
        
        /// <summary>
        /// Get information on a public or private channel, IM, or multi-person IM.
        /// </summary>
        [Obsolete("Use GetConversationById instead")]
        Task<Hub> GetHubById(string hubId);

        /// <summary>
        /// Find hub with matching name.
        /// </summary>
        /// <param name="channel">Channel, group or IM name, with leading # or @ symbol as appropriate.</param>
        [Obsolete("Use GetConversationByName instead")]
        Task<Hub> GetHubByName(string channel);

        /// <summary>
        /// Find channel by name, with or without leading #.
        /// </summary>
        [Obsolete("Use GetConversationByName instead")]
        Task<Hub> GetChannelByName(string name);

        /// <summary>
        /// Find private group by name.
        /// </summary>
        [Obsolete("Use GetConversationByName instead")]
        Task<Hub> GetGroupByName(string name);

        /// <summary>
        /// Find user by name, with or without leading @.
        /// </summary>
        [Obsolete("Use GetConversationByName instead")]
        Task<Im> GetImByName(string username);

        /// <summary>
        /// Get and open Im by user ID.
        /// </summary>
        [Obsolete("Use GetConversationByUserId instead")]
        Task<Im> GetImByUserId(string userId);

        /// <summary>
        /// Get full list of public channels.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        Task<IReadOnlyList<Channel>> GetChannels();

        /// <summary>
        /// Get list of private groups that the bot is in.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        Task<IReadOnlyList<Channel>> GetGroups();

        /// <summary>
        /// Get list of multi-person IMs that the bot is in.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        Task<IReadOnlyList<Channel>> GetMpIms();

        /// <summary>
        /// Get list of IMs that have been opened with the bot.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        Task<IReadOnlyList<Im>> GetIms();
        
        #endregion
    }

    public class SlackBot : ISlackBot, IDisposable
    {
        private readonly ISlackRtmClient _rtm;
        private readonly ISlackApiClient _api;
        private readonly IScheduler _scheduler;
        private readonly ConcurrentQueue<IMessageHandler> _handlers = new ConcurrentQueue<IMessageHandler>();
        
        private readonly ConcurrentValue<Task> _conversationsFetched = new ConcurrentValue<Task>();
        private readonly ConcurrentDictionary<string, Task<Conversation>> _conversations = new ConcurrentDictionary<string, Task<Conversation>>();
        
        private readonly ConcurrentDictionary<string, Task<User>> _users = new ConcurrentDictionary<string, Task<User>>();
        private readonly ConcurrentDictionary<string, Task<BotInfo>> _bots = new ConcurrentDictionary<string, Task<BotInfo>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<User>>> _allUsers = new ConcurrentValue<Task<IReadOnlyList<User>>>();
        
        private readonly SyncedSubject<IMessage> _incomingMessages = new SyncedSubject<IMessage>();
        private readonly SyncedSubject<BotMessage> _outgoingMessages = new SyncedSubject<BotMessage>();
        private IObservable<PostedMessage> _sentMessages;
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
                .Where(m => m.GetType() == typeof(MessageEvent) || m.GetType() == typeof(Events.BotMessage))
                .Where(m => m.User != Id)
                .SelectMany(CreateSlackMessage);
            _outgoingWithMiddlewareApplied = _outgoingMessages
                .LimitFrequency(TimeSpan.FromSeconds(1), m => m.CancellationToken ?? CancellationToken.None, _scheduler);
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
            _sentMessages = _outgoingWithMiddlewareApplied
                .Select(m => new PostedMessage { Message = m, Post = PostMessage(m) })
                .Retry()
                .Publish()
                .RefCount();
            _outgoingSubscription = _sentMessages.Subscribe();

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
            var user = GetMessageUser(message);
            var conversation = GetConversationById(message.Channel);
            var hub = GetHubById(message.Channel);

            return new SlackMessage(this)
                {
                    Ts = message.Ts,
                    ThreadTs = message.ThreadTs,
                    Text = message.Text,
                    User = await user.ConfigureAwait(false),
                    Conversation = await conversation.ConfigureAwait(false),
                    Hub = await hub.ConfigureAwait(false),
                    Attachments = message.Attachments
                };
        }

        private async Task<User> GetMessageUser(MessageEvent message)
        {
            var userId = message.User;
            
            if (userId == null && message is Events.BotMessage b)
            {
                var botInfo = await GetBotUserById(b.BotId).ConfigureAwait(false);
                userId = botInfo.UserId;
            }

            return await GetUserById(userId).ConfigureAwait(false);
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
        /// Retrieve information about a conversation.
        /// </summary>
        public async Task<Conversation> GetConversationById(string conversationId) =>
            string.IsNullOrEmpty(conversationId)
                ? null
                : await _conversations.GetOrAdd(conversationId, FetchConversation).ConfigureAwait(false);

        private async Task<Conversation> FetchConversation(string conversationId) =>
            await _api.Conversations.Info(conversationId).NullIfNotFound().ConfigureAwait(false);

        /// <summary>
        /// Find conversation with matching name.
        /// </summary>
        /// <param name="conversationName">Channel, group or IM name, with leading # or @ symbol or without.</param>
        public async Task<Conversation> GetConversationByName(string conversationName) =>
            conversationName.FirstOrDefault() == '@'
                ? await GetConversationByUserId((await GetUserByName(conversationName).ConfigureAwait(false)).Id).ConfigureAwait(false)
                : await FindConversation(c => c.Name == conversationName.TrimStart('#')).ConfigureAwait(false);
        
        /// <summary>
        /// Get and open Im by user ID.
        /// </summary>
        public async Task<Conversation> GetConversationByUserId(string userId) =>
            userId == null ? null
                : await FindConversation(c => c.IsIm && c.User == userId).ConfigureAwait(false)
                ?? await OpenAndCacheImConversation(userId).ConfigureAwait(false);

        private async Task<Conversation> OpenAndCacheImConversation(string userId)
        {
            var channel = await OpenImConversation(userId).ConfigureAwait(false);
            if (channel != null)
                _conversations[channel.Id] = Task.FromResult(channel);
            return channel;
        }

        private async Task<Conversation> FindConversation(Func<Conversation, bool> predicate) =>
            (await GetConversations().ConfigureAwait(false)).FirstOrDefault(predicate);

        private async Task<Conversation> OpenImConversation(string userId) =>
            (await _api.Conversations.OpenAndReturnInfo(new[] { userId }).NullIfNotFound().ConfigureAwait(false))?.Channel;

        /// <summary>
        /// Get full list conversations the bot user has access to.
        /// </summary>
        public async Task<IReadOnlyCollection<Conversation>> GetConversations()
        {
            await _conversationsFetched.GetOrCreateValue(FetchConversations).ConfigureAwait(false);
            return (await _conversations.Values
                    .ToObservable()
                    .SelectMany(c => c)
                    .ToList()
                    .ToTask().ConfigureAwait(false))
                .ToList();
        }

        private async Task FetchConversations()
        {
            string cursor = null;
            
            do
            {
                var response = await _api.Conversations.List(cursor: cursor).ConfigureAwait(false);
                
                foreach (var conversation in response.Channels) 
                    _conversations[conversation.Id] = Task.FromResult(conversation);

                cursor = response.ResponseMetadata.NextCursor;
            } while (cursor != null);
        }

        /// <summary>
        /// Get user information.
        /// </summary>
        public async Task<User> GetUserById(string userId) =>
            string.IsNullOrEmpty(userId)
                ? null
                : await _users.GetOrAdd(userId, _ => _api.Users.Info(userId).NullIfNotFound()).ConfigureAwait(false);

        /// <summary>
        /// Get bot user information.
        /// </summary>
        public async Task<BotInfo> GetBotUserById(string botId) =>
            string.IsNullOrEmpty(botId)
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
        public async Task Send(BotMessage message, CancellationToken? cancellationToken = null)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                message.CancellationToken ?? CancellationToken.None, 
                cancellationToken ?? CancellationToken.None);
            message.CancellationToken = linkedTokenSource.Token;

            var sent = _sentMessages.FirstOrDefaultAsync(m => m.Message == message)
                .SelectMany(m => m.Post)
                .ToTask(linkedTokenSource.Token);

            _outgoingMessages.OnNext(message);

            await sent.ConfigureAwait(false);
        }

        private async Task<PostMessageResponse> PostMessage(BotMessage message) =>
            await _api.Chat.PostMessage(new Message
                {
                    Channel = message.Conversation != null
                        ? await message.Conversation.ConversationId(this).ConfigureAwait(false)
                        : message.ReplyTo?.Conversation?.Id,
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
                }, message.CancellationToken).ConfigureAwait(false);

        private async Task<bool> ReplyingInDifferentHub(BotMessage message) => 
            message.Conversation != null 
            && await message.Conversation.ConversationId(this).ConfigureAwait(false) != message.ReplyTo?.Conversation.Id;

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
            _conversationsFetched.Clear();
            _conversations.Clear();
            _users.Clear();
            _allUsers.Clear();
            
            _hubs.Clear();
            _channels.Clear();
            _groups.Clear();
            _mpims.Clear();
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
        
        #region Hubs

        private readonly ConcurrentDictionary<string, Task<Hub>> _hubs = new ConcurrentDictionary<string, Task<Hub>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Channel>>> _channels = new ConcurrentValue<Task<IReadOnlyList<Channel>>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Channel>>> _groups = new ConcurrentValue<Task<IReadOnlyList<Channel>>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Channel>>> _mpims = new ConcurrentValue<Task<IReadOnlyList<Channel>>>();
        private readonly ConcurrentValue<Task<IReadOnlyList<Im>>> _ims = new ConcurrentValue<Task<IReadOnlyList<Im>>>();

        /// <summary>
        /// Get information on a public or private channel, IM, or multi-person IM.
        /// </summary>
        [Obsolete("Use GetConversationById instead")]
        public async Task<Hub> GetHubById(string hubId) =>
            string.IsNullOrEmpty(hubId)
                ? null
                : await _hubs.GetOrAdd(hubId, FetchHub).ConfigureAwait(false);

        private async Task<Hub> FetchHub(string hubId) =>
            (await GetConversationById(hubId).ConfigureAwait(false))?.ToHub();
            
        /// <summary>
        /// Find hub with matching name.
        /// </summary>
        /// <param name="channel">Channel, group or IM name, with leading # or @ symbol as appropriate.</param>
        [Obsolete("Use GetConversationByName instead")]
        public async Task<Hub> GetHubByName(string channel) =>
              channel.FirstOrDefault() == '#' ? await GetChannelByName(channel).ConfigureAwait(false)
            : channel.FirstOrDefault() == '@' ? await GetImByName(channel).ConfigureAwait(false)
            : await GetGroupByName(channel).ConfigureAwait(false);

        /// <summary>
        /// Find channel by name, with or without leading #.
        /// </summary>
        [Obsolete("Use GetConversationByName instead")]
        public async Task<Hub> GetChannelByName(string name) =>
            await FindCachedHub<Channel>(h => h.IsChannel && h.Name == WithoutLeadingHash(name)).ConfigureAwait(false)
            ?? (await GetChannels().ConfigureAwait(false))
                .FirstOrDefault(c => c.Name == WithoutLeadingHash(name));

        private static string WithoutLeadingHash(string name) => name.TrimStart('#');

        /// <summary>
        /// Find private group by name.
        /// </summary>
        [Obsolete("Use GetConversationByName instead")]
        public async Task<Hub> GetGroupByName(string name) =>
            await FindCachedHub<Channel>(h => h.IsGroup && h.Name == name).ConfigureAwait(false)
            ?? (await GetGroups().ConfigureAwait(false))
                .FirstOrDefault(g => g.Name == name);

        /// <summary>
        /// Get an open Im by user name, with or without leading @.
        /// </summary>
        [Obsolete("Use GetConversationByName instead")]
        public async Task<Im> GetImByName(string username) =>
            await GetImByUserId(
                (await GetUserByName(username).ConfigureAwait(false))?.Id).ConfigureAwait(false);

        /// <summary>
        /// Get and open Im by user ID.
        /// </summary>
        [Obsolete("Use GetConversationByUserId instead")]
        public async Task<Im> GetImByUserId(string userId) =>
            userId == null ? null
                : await FindCachedHub<Im>(h => h.User == userId).ConfigureAwait(false)
                    ?? await OpenAndCacheIm(userId).ConfigureAwait(false);

        private Task<T> FindCachedHub<T>(Func<T, bool> predicate) where T : Hub => _hubs.Values.FirstOrDefaultAsync(predicate);
        
        private async Task<Im> OpenAndCacheIm(string userId)
        {
            var im = await OpenIm(userId).ConfigureAwait(false);
            if (im != null)
                _hubs[im.Id] = Task.FromResult((Hub)im);
            return im;
        }
        
        private async Task<Im> OpenIm(string userId) => 
            (await OpenImConversation(userId).ConfigureAwait(false))?.ToIm();

        /// <summary>
        /// Get full list of public channels.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Channel>> GetChannels() => _channels.GetOrCreateValue(FetchChannels);

        private async Task<IReadOnlyList<Channel>> FetchChannels() => CacheHubs(
            (await GetConversations().ConfigureAwait(false))
            .Where(c => c.IsChannel)
            .Select(ConversationConversion.ToChannel)
            .ToList());

        /// <summary>
        /// Get list of private groups that the bot is in.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Channel>> GetGroups() => _groups.GetOrCreateValue(FetchGroups);
        
        private async Task<IReadOnlyList<Channel>> FetchGroups() => CacheHubs(
            (await GetConversations().ConfigureAwait(false))
            .Where(c => c.IsGroup)
            .Select(ConversationConversion.ToChannel)
            .ToList());

        /// <summary>
        /// Get list of multi-person IMs that the bot is in.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Channel>> GetMpIms() => _mpims.GetOrCreateValue(FetchMpims);
        
        private async Task<IReadOnlyList<Channel>> FetchMpims() => CacheHubs(
            (await GetConversations().ConfigureAwait(false))
            .Where(c => c.IsMpim)
            .Select(ConversationConversion.ToChannel)
            .ToList());

        private IReadOnlyList<Channel> CacheHubs(IReadOnlyList<Channel> channels)
        {
            foreach (var channel in channels)
                _hubs[channel.Id] = Task.FromResult((Hub)channel);
            return channels;
        }

        /// <summary>
        /// Get list of IMs that have been opened with the bot.
        /// </summary>
        [Obsolete("Use GetConversations instead")]
        public Task<IReadOnlyList<Im>> GetIms() => _ims.GetOrCreateValue(FetchIms);

        private async Task<IReadOnlyList<Im>> FetchIms() =>
            (await GetConversations().ConfigureAwait(false))
            .Where(c => c.IsIm)
            .Select(ConversationConversion.ToIm)
            .ToList();
        
        #endregion
    }

    class PostedMessage
    {
        public BotMessage Message { get; set; }
        public Task<PostMessageResponse> Post { get; set; }
    }
}
