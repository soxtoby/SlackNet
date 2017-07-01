using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events.Messages;
using SlackNet.Objects;
using SlackNet.WebApi.Responses;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class GroupsApi
    {
        private readonly WebApiClient _client;
        public GroupsApi(WebApiClient client) => _client = client;

        /// <summary>
        /// Archives a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to archive.</param>
        /// <param name="cancellationToken"></param>
        public Task Archive(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("groups.archive", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Closes a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to close.</param>
        /// <param name="cancellationToken"></param>
        public Task Close(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("groups.close", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Creates a private channel.
        /// </summary>
        /// <param name="name">Name of private channel to create.</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Group> Create(string name, bool validate = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupResponse>("groups.create", new Args
                {
                    { "name", name },
                    { "validate", validate }
                }, cancellationToken).ConfigureAwait(false))
            .Group;

        /// <summary>
        /// Takes an existing private channel, renames and archives it, and creates a new private channel with the same name and members.
        /// See https://api.slack.com/methods/groups.createChild for more information.
        /// </summary>
        /// <param name="channelId">Private channel to clone and archive.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Group> CreateChild(string channelId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupResponse>("groups.createChild", new Args { { "channel", channelId } }, cancellationToken).ConfigureAwait(false)).Group;

        /// <summary>
        /// Returns a portion of message events from the specified private channel.
        /// To read the entire history for a private channel, call the method with no latest or oldest arguments, and then continue paging.
        /// To retrieve a single message, specify its ts value as latest, set inclusive to true, and dial your count down to 1.
        /// See https://api.slack.com/methods/groups.history for more information.
        /// </summary>
        /// <param name="channelId">Private channel to fetch history for.</param>
        /// <param name="latestTs">End of time range of messages to include in results. Defaults to now.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results. Defaults to first message.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results.</param>
        /// <param name="count">Number of messages to return, between 1 and 1000.</param>
        /// <param name="unreads">Include <see cref="ChannelHistoryResponse.UnreadCountDisplay"/> in the output?</param>
        /// <param name="cancellationToken"></param>
        public Task<ChannelHistoryResponse> History(
            string channelId, 
            string latestTs = null, 
            string oldestTs = null, 
            bool inclusive = false, 
            int count = 100, 
            bool unreads = false, 
            CancellationToken? cancellationToken = null
        ) =>
            _client.Get<ChannelHistoryResponse>("groups.history", new Args
                    {
                        { "channel", channelId },
                        { "latest", latestTs },
                        { "oldest", oldestTs },
                        { "inclusive", inclusive },
                        { "count", count },
                        { "unreads", unreads }
                    },
                cancellationToken);

        /// <summary>
        /// Returns information about a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to get info on.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Group> Info(string channelId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupResponse>("groups.info", new Args { { "channel", channelId } }, cancellationToken).ConfigureAwait(false)).Group;

        /// <summary>
        /// Used to invite a user to a private channel. The calling user must be a member of the private channel.
        /// To invite a new member to a private channel without giving them access to the archives of the private channel,
        /// call the <see cref="CreateChild(string,System.Nullable{System.Threading.CancellationToken})"/> method before inviting.
        /// </summary>
        /// <param name="channelId">Private channel to invite user to.</param>
        /// <param name="userId">User to invite.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Group> Invite(string channelId, string userId, CancellationToken? cancellationToken) =>
            (await _client.Get<GroupResponse>("groups.invite", new Args
                {
                    { "channel", channelId },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Group;

        /// <summary>
        /// Allows a user to remove another member from a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to remove user from.</param>
        /// <param name="userId">User to remove from private channel.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Kick(string channelId, string userId, CancellationToken? cancellationToken) =>
            _client.Get("groups.kick", new Args
                {
                    { "channel", channelId },
                    { "user", userId }
                }, cancellationToken);

        /// <summary>
        /// Used to leave a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to leave.</param>
        /// <param name="cancellationToken"></param>
        public Task Leave(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("groups.leave", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Returns a list of private channels in the team that the caller is in and archived groups that the caller was in. 
        /// The list of (non-deactivated) members in each private channel is also returned.
        /// </summary>
        /// <param name="excludeArchived">Don't return archived private channels.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Group>> List(bool excludeArchived = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupListResponse>("groups.list", new Args { { "exclude_archived", excludeArchived } }, cancellationToken).ConfigureAwait(false)).Groups;

        /// <summary>
        /// Moves the read cursor in a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to set reading cursor in.</param>
        /// <param name="ts">Timestamp of the most recently seen message.</param>
        /// <param name="cancellationToken"></param>
        public Task Mark(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("groups.mark", new Args { { "channel", channelId }, { "ts", ts } }, cancellationToken);

        /// <summary>
        /// Opens a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to open.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Open(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("groups.open", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Renames a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to rename</param>
        /// <param name="name">New name for private channel.</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Group> Rename(string channelId, string name, bool validate = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupRenameResponse>("groups.rename", new Args
                {
                    { "channel", channelId },
                    { "name", name },
                    { "validate", validate }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Returns an entire thread (a message plus all the messages in reply to it).
        /// </summary>
        /// <param name="channelId">Private channel to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Message>> Replies(string channelId, string threadTs, CancellationToken? cancellationToken = null) =>
            (await _client.Get<RepliesResponse>("groups.replies", new Args
                {
                    { "channel", channelId },
                    { "thread_ts", threadTs }
                }, cancellationToken).ConfigureAwait(false))
            .Messages;

        /// <summary>
        /// Used to change the purpose of a private channel. The calling user must be a member of the private channel.
        /// </summary>
        /// <param name="channelId">Private channel to set the purpose of</param>
        /// <param name="purpose">The new purpose.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The group's new purpose.</returns>
        public async Task<string> SetPurpose(string channelId, string purpose, CancellationToken? cancellationToken = null) =>
            (await _client.Get<PurposeResponse>("channels.setPurpose", new Args
                {
                    { "channel", channelId },
                    { "purpose", purpose }
                }, cancellationToken).ConfigureAwait(false))
            .Purpose;

        /// <summary>
        /// Used to change the topic of a private channel. The calling user must be a member of the private channel.
        /// </summary>
        /// <param name="channelId">Private channel to set the topic of.</param>
        /// <param name="topic">The new topic.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The private channel's new topic.</returns>
        public async Task<string> SetTopic(string channelId, string topic, CancellationToken? cancellationToken = null) =>
            (await _client.Get<TopicResponse>("channels.setTopic", new Args
                {
                    { "channel", channelId },
                    { "topic", topic }
                }, cancellationToken).ConfigureAwait(false))
            .Topic;

        /// <summary>
        /// Unarchives a private channel.
        /// </summary>
        /// <param name="channelId">Private channel to unarchive.</param>
        /// <param name="cancellationToken"></param>
        public Task Unarchive(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("channels.unarchive", new Args { { "channel", channelId } }, cancellationToken);
    }
}