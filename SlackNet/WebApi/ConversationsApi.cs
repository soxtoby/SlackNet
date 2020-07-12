using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IConversationsApi
    {
        /// <summary>
        /// Archives a conversation. Not all types of conversations can be archived.
        /// </summary>
        /// <param name="channelId">ID of conversation to archive.</param>
        /// <param name="cancellationToken"></param>
        Task Archive(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Closes a direct message or multi-person direct message.
        /// </summary>
        /// <param name="channelId">Conversation to close.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Close(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Initiates a public or private channel-based conversation.
        /// Use <c>Open</c> to initiate or resume a direct message or multi-person direct message.
        /// </summary>
        /// <param name="name">Name of the public or private channel to create.</param>
        /// <param name="isPrivate">Create a private channel instead of a public one.</param>
        /// <param name="cancellationToken"></param>
        Task<Conversation> Create(string name, bool isPrivate, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Fetches a conversation's history of messages and events.
        /// </summary>
        /// <param name="channelId">Conversation ID to fetch history for.</param>
        /// <param name="latestTs">End of time range of messages to include in results.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results only when either timestamp is specified.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationHistoryResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<ConversationHistoryResponse> History(string channelId, string latestTs = null, string oldestTs = null, bool inclusive = false, int limit = 100, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Retrieve information about a conversation.
        /// </summary>
        /// <param name="channelId">Conversation ID to learn more about.</param>
        /// <param name="includeLocale">Set this to true to receive the locale for this conversation.</param>
        /// <param name="includeNumMembers"></param>
        /// <param name="cancellationToken"></param>
        Task<Conversation> Info(string channelId, bool includeLocale = false, bool includeNumMembers = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Invites users to a channel.
        /// </summary>
        /// <param name="channelId">The ID of the public or private channel to invite user(s) to.</param>
        /// <param name="userIds">A comma separated list of user IDs. Up to 30 users may be listed.</param>
        /// <param name="cancellationToken"></param>
        Task<Conversation> Invite(string channelId, IEnumerable<string> userIds, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Joins an existing conversation.
        /// </summary>
        /// <param name="channelId">ID of conversation to join.</param>
        /// <param name="cancellationToken"></param>
        Task<ConversationJoinResponse> Join(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a user from a conversation.
        /// </summary>
        /// <param name="channelId">ID of conversation to remove user from.</param>
        /// <param name="userId">User ID to be removed.</param>
        /// <param name="cancellationToken"></param>
        Task Kick(string channelId, string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Leaves a conversation.
        /// </summary>
        /// <param name="channelId">Conversation to leave.</param>
        /// <param name="cancellationToken"></param>
        Task Leave(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Lists all channels in a Slack team.
        /// </summary>
        /// <param name="excludeArchived">Set to True to exclude archived channels from the list.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the list hasn't been reached. Must be an integer no larger than 1000.</param>
        /// <param name="types">Types of conversations to include. Default is <see cref="ConversationType.PublicChannel"/>.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationListResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<ConversationListResponse> List(bool excludeArchived = false, int limit = 100, IEnumerable<ConversationType> types = null, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Sets the read cursor in a channel.
        /// </summary>
        /// <param name="channelId">Channel or conversation to set the read cursor for.</param>
        /// <param name="messageTs">Unique identifier of message you want marked as most recently seen in this conversation.</param>
        /// <param name="cancellationToken"></param>
        Task Mark(string channelId, string messageTs, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Retrieve members of a conversation.
        /// </summary>
        /// <param name="channelId">ID of the conversation to retrieve members for.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationMembersResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<ConversationMembersResponse> Members(string channelId, int limit = 100, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="Create"/> for public or private channels.
        /// </summary>
        /// <param name="channelId">Resume a conversation by supplying an im or mpim's ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The opened channel's ID</returns>
        Task<string> Open(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="Create"/> for public or private channels.
        /// </summary>
        /// <param name="userIds">List of users. If only one user is included, this creates a 1:1 DM. The ordering of the users is preserved whenever a multi-person direct message is returned.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The opened channel's ID</returns>
        Task<string> Open(IEnumerable<string> userIds, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="Create"/> for public or private channels.
        /// </summary>
        /// <param name="channelId">Resume a conversation by supplying an im or mpim's ID.</param>
        /// <param name="cancellationToken"></param>
        Task<ConversationOpenResponse> OpenAndReturnInfo(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="Create"/> for public or private channels.
        /// </summary>
        /// <param name="userIds">List of users. If only one user is included, this creates a 1:1 DM. The ordering of the users is preserved whenever a multi-person direct message is returned.</param>
        /// <param name="cancellationToken"></param>
        Task<ConversationOpenResponse> OpenAndReturnInfo(IEnumerable<string> userIds, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Renames a conversation.
        /// </summary>
        /// <param name="channelId">ID of conversation to rename.</param>
        /// <param name="name">New name for conversation.</param>
        /// <param name="cancellationToken"></param>
        Task<Conversation> Rename(string channelId, string name, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Retrieve a thread of messages posted to a conversation.
        /// </summary>
        /// <param name="channelId">Conversation ID to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="latestTs">End of time range of messages to include in results.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results only when either timestamp is specified.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationMessagesResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<ConversationMessagesResponse> Replies(string channelId, string threadTs, string latestTs = null, string oldestTs = null, bool inclusive = false, int limit = 10, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Sets the purpose for a conversation.
        /// </summary>
        /// <param name="channelId">Conversation to set the purpose of.</param>
        /// <param name="purpose">A new, specialer purpose.</param>
        /// <param name="cancellationToken"></param>
        Task<string> SetPurpose(string channelId, string purpose, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Sets the topic for a conversation.
        /// </summary>
        /// <param name="channelId">Conversation to set the topic of.</param>
        /// <param name="topic">The new topic string. Does not support formatting or linkification.</param>
        /// <param name="cancellationToken"></param>
        Task<string> SetTopic(string channelId, string topic, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Reverses conversation archival.
        /// </summary>
        /// <param name="channelId">ID of conversation to unarchive.</param>
        /// <param name="cancellationToken"></param>
        Task Unarchive(string channelId, CancellationToken? cancellationToken = null);
    }

    public class ConversationsApi : IConversationsApi
    {
        private readonly ISlackApiClient _client;
        public ConversationsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Archives a conversation. Not all types of conversations can be archived.
        /// </summary>
        /// <param name="channelId">ID of conversation to archive.</param>
        /// <param name="cancellationToken"></param>
        public Task Archive(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post("conversations.archive", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Closes a direct message or multi-person direct message.
        /// </summary>
        /// <param name="channelId">Conversation to close.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Close(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post("conversations.close", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Initiates a public or private channel-based conversation.
        /// Use <c>Open</c> to initiate or resume a direct message or multi-person direct message.
        /// </summary>
        /// <param name="name">Name of the public or private channel to create.</param>
        /// <param name="isPrivate">Create a private channel instead of a public one.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Conversation> Create(string name, bool isPrivate, CancellationToken? cancellationToken = null) =>
            (await _client.Post<ConversationResponse>("conversations.create", new Args
                {
                    { "name", name },
                    { "is_private", isPrivate }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Fetches a conversation's history of messages and events.
        /// </summary>
        /// <param name="channelId">Conversation ID to fetch history for.</param>
        /// <param name="latestTs">End of time range of messages to include in results.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results only when either timestamp is specified.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationHistoryResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationHistoryResponse> History(string channelId, string latestTs = null, string oldestTs = null, bool inclusive = false, int limit = 100, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<ConversationHistoryResponse>("conversations.history", new Args
                {
                    { "channel", channelId },
                    { "cursor", cursor },
                    { "inclusive", inclusive },
                    { "latest", latestTs },
                    { "limit", limit },
                    { "oldest", oldestTs }
                }, cancellationToken);

        /// <summary>
        /// Retrieve information about a conversation.
        /// </summary>
        /// <param name="channelId">Conversation ID to learn more about.</param>
        /// <param name="includeLocale">Set this to true to receive the locale for this conversation.</param>
        /// <param name="includeNumMembers">Set to true to include the member count for the specified conversation.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Conversation> Info(string channelId, bool includeLocale = false, bool includeNumMembers = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ConversationResponse>("conversations.info", new Args
                {
                    { "channel", channelId },
                    { "include_locale", includeLocale },
                    { "include_num_members", includeNumMembers }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Invites users to a channel.
        /// </summary>
        /// <param name="channelId">The ID of the public or private channel to invite user(s) to.</param>
        /// <param name="userIds">A comma separated list of user IDs. Up to 30 users may be listed.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Conversation> Invite(string channelId, IEnumerable<string> userIds, CancellationToken? cancellationToken = null) =>
            (await _client.Post<ConversationResponse>("conversations.invite", new Args
                {
                    { "channel", channelId },
                    { "users", userIds }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Joins an existing conversation.
        /// </summary>
        /// <param name="channelId">ID of conversation to join.</param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationJoinResponse> Join(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post<ConversationJoinResponse>("conversations.join", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Removes a user from a conversation.
        /// </summary>
        /// <param name="channelId">ID of conversation to remove user from.</param>
        /// <param name="userId">User ID to be removed.</param>
        /// <param name="cancellationToken"></param>
        public Task Kick(string channelId, string userId, CancellationToken? cancellationToken = null) =>
            _client.Post("conversations.kick", new Args
                {
                    { "channel", channelId },
                    { "user", userId }
                }, cancellationToken);

        /// <summary>
        /// Leaves a conversation.
        /// </summary>
        /// <param name="channelId">Conversation to leave.</param>
        /// <param name="cancellationToken"></param>
        public Task Leave(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post("conversations.leave", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Lists all channels in a Slack team.
        /// </summary>
        /// <param name="excludeArchived">Set to True to exclude archived channels from the list.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the list hasn't been reached. Must be an integer no larger than 1000.</param>
        /// <param name="types">Types of conversations to include. Default is <see cref="ConversationType.PublicChannel"/>.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationListResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationListResponse> List(bool excludeArchived = false, int limit = 100, IEnumerable<ConversationType> types = null, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<ConversationListResponse>("conversations.list", new Args
                {
                    { "cursor", cursor },
                    { "exclude_archived", excludeArchived },
                    { "limit", limit },
                    { "types", types }
                }, cancellationToken);

        /// <summary>
        /// Sets the read cursor in a channel.
        /// </summary>
        /// <param name="channelId">Channel or conversation to set the read cursor for.</param>
        /// <param name="messageTs">Unique identifier of message you want marked as most recently seen in this conversation.</param>
        /// <param name="cancellationToken"></param>
        public Task Mark(string channelId, string messageTs, CancellationToken? cancellationToken = null) =>
            _client.Post("conversations.mark", new Args
                {
                    { "channel", channelId },
                    { "ts", messageTs }
                }, cancellationToken);

        /// <summary>
        /// Retrieve members of a conversation.
        /// </summary>
        /// <param name="channelId">ID of the conversation to retrieve members for.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationMembersResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationMembersResponse> Members(string channelId, int limit = 100, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<ConversationMembersResponse>("conversations.members", new Args
                {
                    { "channel", channelId },
                    { "cursor", cursor },
                    { "limit", limit }
                }, cancellationToken);

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="IConversationsApi.Create"/> for public or private channels.
        /// </summary>
        /// <param name="channelId">Resume a conversation by supplying an im or mpim's ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The opened channel's ID</returns>
        public async Task<string> Open(string channelId, CancellationToken? cancellationToken = null) => 
            (await Open<ConversationIdResponse>(false, channelId, null, cancellationToken).ConfigureAwait(false)).Channel.Id;

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="IConversationsApi.Create"/> for public or private channels.
        /// </summary>
        /// <param name="userIds">List of users. If only one user is included, this creates a 1:1 DM. The ordering of the users is preserved whenever a multi-person direct message is returned.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The opened channel's ID</returns>
        public async Task<string> Open(IEnumerable<string> userIds, CancellationToken? cancellationToken = null) => 
            (await Open<ConversationIdResponse>(false, null, userIds, cancellationToken).ConfigureAwait(false)).Channel.Id;

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="Create"/> for public or private channels.
        /// </summary>
        /// <param name="channelId">Resume a conversation by supplying an im or mpim's ID.</param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationOpenResponse> OpenAndReturnInfo(string channelId, CancellationToken? cancellationToken = null) => 
            Open<ConversationOpenResponse>(true, channelId, null, cancellationToken);

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// Use <see cref="Create"/> for public or private channels.
        /// </summary>
        /// <param name="userIds">List of users. If only one user is included, this creates a 1:1 DM. The ordering of the users is preserved whenever a multi-person direct message is returned.</param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationOpenResponse> OpenAndReturnInfo(IEnumerable<string> userIds, CancellationToken? cancellationToken = null) => 
            Open<ConversationOpenResponse>(true, null, userIds, cancellationToken);

        private Task<T> Open<T>(bool returnIm, string channelId = null, IEnumerable<string> userIds = null, CancellationToken? cancellationToken = null) where T : class =>
            _client.Post<T>("conversations.open", new Args
                {
                    { "channel", channelId },
                    { "return_im", returnIm },
                    { "users", userIds }
                }, cancellationToken);

        /// <summary>
        /// Renames a conversation.
        /// </summary>
        /// <param name="channelId">ID of conversation to rename.</param>
        /// <param name="name">New name for conversation.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Conversation> Rename(string channelId, string name, CancellationToken? cancellationToken = null) =>
            (await _client.Post<ConversationResponse>("conversations.rename", new Args
                {
                    { "channel", channelId },
                    { "name", name }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Retrieve a thread of messages posted to a conversation.
        /// </summary>
        /// <param name="channelId">Conversation ID to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="latestTs">End of time range of messages to include in results.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results only when either timestamp is specified.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="ConversationMessagesResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<ConversationMessagesResponse> Replies(string channelId, string threadTs, string latestTs = null, string oldestTs = null, bool inclusive = false, int limit = 10, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<ConversationMessagesResponse>("conversations.replies", new Args
                {
                    { "channel", channelId },
                    { "ts", threadTs },
                    { "cursor", cursor },
                    { "inclusive", inclusive },
                    { "latest", latestTs },
                    { "limit", limit },
                    { "oldest", oldestTs }
                }, cancellationToken);

        /// <summary>
        /// Sets the purpose for a conversation.
        /// </summary>
        /// <param name="channelId">Conversation to set the purpose of.</param>
        /// <param name="purpose">A new, specialer purpose.</param>
        /// <param name="cancellationToken"></param>
        public async Task<string> SetPurpose(string channelId, string purpose, CancellationToken? cancellationToken = null) =>
            (await _client.Post<PurposeResponse>("conversations.setPurpose", new Args
                {
                    { "channel", channelId },
                    { "purpose", purpose }
                }, cancellationToken).ConfigureAwait(false))
            .Purpose;

        /// <summary>
        /// Sets the topic for a conversation.
        /// </summary>
        /// <param name="channelId">Conversation to set the topic of.</param>
        /// <param name="topic">The new topic string. Does not support formatting or linkification.</param>
        /// <param name="cancellationToken"></param>
        public async Task<string> SetTopic(string channelId, string topic, CancellationToken? cancellationToken = null) =>
            (await _client.Post<TopicResponse>("conversations.setTopic", new Args
                {
                    { "channel", channelId },
                    { "topic", topic }
                }, cancellationToken).ConfigureAwait(false))
            .Topic;

        /// <summary>
        /// Reverses conversation archival.
        /// </summary>
        /// <param name="channelId">ID of conversation to unarchive.</param>
        /// <param name="cancellationToken"></param>
        public Task Unarchive(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post("conversations.unarchive", new Args { { "channel", channelId } }, cancellationToken);
    }
}