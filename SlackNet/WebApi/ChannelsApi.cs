using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IChannelsApi
    {
        /// <summary>
        /// Archives a channel.
        /// </summary>
        /// <param name="channelId">Channel to archive.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Archive(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Used to create a channel.
        /// To create a private channel, use <see cref="GroupsApi.Create"/>
        /// </summary>
        /// <param name="name">Name of channel to create</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        Task<Channel> Create(string name, bool validate = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a portion of message events from the specified public channel.
        /// To read the entire history for a channel, call the method with no latest or oldest arguments, and then continue paging.
        /// To retrieve a single message, specify its ts value as latest, set inclusive to true, and dial your count down to 1.
        /// See https://api.slack.com/methods/channels.history for more information.
        /// </summary>
        /// <param name="channelId">Channel to fetch history for.</param>
        /// <param name="latestTs">End of time range of messages to include in results. Defaults to now.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results. Defaults to first message.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results.</param>
        /// <param name="count">Number of messages to return, between 1 and 1000.</param>
        /// <param name="unreads">Include <see cref="ChannelHistoryResponse.UnreadCountDisplay"/> in the output?</param>
        /// <param name="cancellationToken"></param>
        Task<ChannelHistoryResponse> History(string channelId, string latestTs = null, string oldestTs = null, bool inclusive = false, int count = 100, bool unreads = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns information about a team channel.
        /// To retrieve information on a private channel, use <see cref="ChannelsApi.Info"/>.
        /// </summary>
        /// <param name="channelId">Channel to get info on</param>
        /// <param name="cancellationToken"></param>
        Task<Channel> Info(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Used to invite a user to a channel. The calling user must be a member of the channel.
        /// </summary>
        /// <param name="channelId">Channel to invite user to.</param>
        /// <param name="userId">User to invite to channel.</param>
        /// <param name="cancellationToken"></param>
        Task<Channel> Invite(string channelId, string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Used to join a channel. If the channel does not exist, it is created.
        /// </summary>
        /// <param name="channelName">Name of channel to join (e.g "#general").</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// If successful, the command returns a channel object, including state information.
        /// If you are already in the channel, the response is slightly different. 
        /// <see cref="ChannelJoinResponse.AlreadyInChannel"/> will be true, and a limited channel object will be returned. 
        /// This allows a client to see that the request to join GeNERaL is the same as the channel #general that the user is already in.
        /// </returns>
        Task<ChannelJoinResponse> Join(string channelName, bool validate = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Allows a user to remove another member from a team channel.
        /// </summary>
        /// <param name="channelId">Channel to remove user from.</param>
        /// <param name="userId">User to remove from channel.</param>
        /// <param name="cancellationToken"></param>
        Task Kick(string channelId, string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Used to leave a channel.
        /// </summary>
        /// <param name="channelId">Channel to leave.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Leave(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of all channels in the team. 
        /// This includes channels the caller is in, channels they are not currently in, and archived channels but does not include private channels. 
        /// The number of (non-deactivated) members in each channel is also returned.
        /// To retrieve a list of private channels, use groups.list.
        /// </summary>
        /// <param name="excludeArchived">Exclude archived channels from the list.</param>
        /// <param name="excludeMembers">Exclude the <see cref="Channel.Members"/> collection from each channel</param>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<Channel>> List(bool excludeArchived = false, bool excludeMembers = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Moves the read cursor in a channel.
        /// </summary>
        /// <param name="channelId">Channel to set reading cursor in.</param>
        /// <param name="ts">Timestamp of the most recently seen message.</param>
        /// <param name="cancellationToken"></param>
        Task Mark(string channelId, string ts, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Renames a team channel.
        /// The only people who can rename a channel are Team Admins, or the person that originally created the channel. 
        /// Others will receive a "not_authorized" error.
        /// </summary>
        /// <param name="channelId">Channel to rename</param>
        /// <param name="name">New name for channel.</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        Task<Channel> Rename(string channelId, string name, bool validate = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns an entire thread (a message plus all the messages in reply to it).
        /// </summary>
        /// <param name="channelId">Channel to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<MessageEvent>> Replies(string channelId, string threadTs, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Used to change the purpose of a channel. The calling user must be a member of the channel.
        /// </summary>
        /// <param name="channelId">Channel to set the purpose of</param>
        /// <param name="purpose">The new purpose.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The channel's new purpose.</returns>
        Task<string> SetPurpose(string channelId, string purpose, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Used to change the topic of a channel. The calling user must be a member of the channel.
        /// </summary>
        /// <param name="channelId">Channel to set the topic of.</param>
        /// <param name="topic">The new topic.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The channel's new topic.</returns>
        Task<string> SetTopic(string channelId, string topic, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Unarchives a channel. The calling user is added to the channel.
        /// </summary>
        /// <param name="channelId">Channel to unarchive.</param>
        /// <param name="cancellationToken"></param>
        Task Unarchive(string channelId, CancellationToken? cancellationToken = null);
    }

    public class ChannelsApi : IChannelsApi
    {
        private readonly ISlackApiClient _client;
        public ChannelsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Archives a channel.
        /// </summary>
        /// <param name="channelId">Channel to archive.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Archive(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("channels.archive", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Used to create a channel.
        /// To create a private channel, use <see cref="GroupsApi.Create"/>
        /// </summary>
        /// <param name="name">Name of channel to create</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Channel> Create(string name, bool validate = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ChannelResponse>("channels.create", new Args
                {
                    { "name", name },
                    { "validate", validate }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Returns a portion of message events from the specified public channel.
        /// To read the entire history for a channel, call the method with no latest or oldest arguments, and then continue paging.
        /// To retrieve a single message, specify its ts value as latest, set inclusive to true, and dial your count down to 1.
        /// See https://api.slack.com/methods/channels.history for more information.
        /// </summary>
        /// <param name="channelId">Channel to fetch history for.</param>
        /// <param name="latestTs">End of time range of messages to include in results. Defaults to now.</param>
        /// <param name="oldestTs">Start of time range of messages to include in results. Defaults to first message.</param>
        /// <param name="inclusive">Include messages with latest or oldest timestamp in results.</param>
        /// <param name="count">Number of messages to return, between 1 and 1000.</param>
        /// <param name="unreads">Include <see cref="ChannelHistoryResponse.UnreadCountDisplay"/> in the output?</param>
        /// <param name="cancellationToken"></param>
        public Task<ChannelHistoryResponse> History(string channelId, string latestTs = null, string oldestTs = null, bool inclusive = false, int count = 100, bool unreads = false, CancellationToken? cancellationToken = null) =>
            _client.Get<ChannelHistoryResponse>("channels.history", new Args
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
        /// Returns information about a team channel.
        /// To retrieve information on a private channel, use <see cref="Info(string,System.Nullable{System.Threading.CancellationToken})"/>.
        /// </summary>
        /// <param name="channelId">Channel to get info on</param>
        /// <param name="cancellationToken"></param>
        public async Task<Channel> Info(string channelId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ChannelResponse>("channels.info", new Args { { "channel", channelId } }, cancellationToken).ConfigureAwait(false)).Channel;

        /// <summary>
        /// Used to invite a user to a channel. The calling user must be a member of the channel.
        /// </summary>
        /// <param name="channelId">Channel to invite user to.</param>
        /// <param name="userId">User to invite to channel.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Channel> Invite(string channelId, string userId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ChannelResponse>("channels.invite", new Args
                {
                    { "channel", channelId },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Used to join a channel. If the channel does not exist, it is created.
        /// </summary>
        /// <param name="channelName">Name of channel to join (e.g "#general").</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// If successful, the command returns a channel object, including state information.
        /// If you are already in the channel, the response is slightly different. 
        /// <see cref="ChannelJoinResponse.AlreadyInChannel"/> will be true, and a limited channel object will be returned. 
        /// This allows a client to see that the request to join GeNERaL is the same as the channel #general that the user is already in.
        /// </returns>
        public Task<ChannelJoinResponse> Join(string channelName, bool validate = false, CancellationToken? cancellationToken = null) =>
            _client.Get<ChannelJoinResponse>("channels.join", new Args
                {
                    { "channel", channelName },
                    { "validate", validate }
                }, cancellationToken);

        /// <summary>
        /// Allows a user to remove another member from a team channel.
        /// </summary>
        /// <param name="channelId">Channel to remove user from.</param>
        /// <param name="userId">User to remove from channel.</param>
        /// <param name="cancellationToken"></param>
        public Task Kick(string channelId, string userId, CancellationToken? cancellationToken = null) =>
            _client.Get("channels.kick", new Args
                {
                    { "channel", channelId },
                    { "user", userId }
                }, cancellationToken);

        /// <summary>
        /// Used to leave a channel.
        /// </summary>
        /// <param name="channelId">Channel to leave.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Leave(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("channels.leave", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Returns a list of all channels in the team. 
        /// This includes channels the caller is in, channels they are not currently in, and archived channels but does not include private channels. 
        /// The number of (non-deactivated) members in each channel is also returned.
        /// To retrieve a list of private channels, use groups.list.
        /// </summary>
        /// <param name="excludeArchived">Exclude archived channels from the list.</param>
        /// <param name="excludeMembers">Exclude the <see cref="Channel.Members"/> collection from each channel</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Channel>> List(bool excludeArchived = false, bool excludeMembers = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ChannelListResponse>("channels.list", new Args
                {
                    { "exclude_archived", excludeArchived },
                    { "exclude_members", excludeMembers }
                }, cancellationToken).ConfigureAwait(false))
            .Channels;

        /// <summary>
        /// Moves the read cursor in a channel.
        /// </summary>
        /// <param name="channelId">Channel to set reading cursor in.</param>
        /// <param name="ts">Timestamp of the most recently seen message.</param>
        /// <param name="cancellationToken"></param>
        public Task Mark(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("channels.mark", new Args
                {
                    { "channel", channelId },
                    { "ts", ts }
                }, cancellationToken);

        /// <summary>
        /// Renames a team channel.
        /// The only people who can rename a channel are Team Admins, or the person that originally created the channel. 
        /// Others will receive a "not_authorized" error.
        /// </summary>
        /// <param name="channelId">Channel to rename</param>
        /// <param name="name">New name for channel.</param>
        /// <param name="validate">Whether to return errors on invalid channel name instead of modifying it to meet the specified criteria.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Channel> Rename(string channelId, string name, bool validate = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ChannelResponse>("channels.rename", new Args
                {
                    { "channel", channelId },
                    { "name", name },
                    { "validate", validate }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Returns an entire thread (a message plus all the messages in reply to it).
        /// </summary>
        /// <param name="channelId">Channel to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<MessageEvent>> Replies(string channelId, string threadTs, CancellationToken? cancellationToken = null) =>
            (await _client.Get<RepliesResponse>("channels.replies", new Args
                {
                    { "channel", channelId },
                    { "thread_ts", threadTs }
                }, cancellationToken).ConfigureAwait(false))
            .Messages;

        /// <summary>
        /// Used to change the purpose of a channel. The calling user must be a member of the channel.
        /// </summary>
        /// <param name="channelId">Channel to set the purpose of</param>
        /// <param name="purpose">The new purpose.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The channel's new purpose.</returns>
        public async Task<string> SetPurpose(string channelId, string purpose, CancellationToken? cancellationToken = null) =>
            (await _client.Get<PurposeResponse>("channels.setPurpose", new Args
                {
                    { "channel", channelId },
                    { "purpose", purpose }
                }, cancellationToken).ConfigureAwait(false))
            .Purpose;

        /// <summary>
        /// Used to change the topic of a channel. The calling user must be a member of the channel.
        /// </summary>
        /// <param name="channelId">Channel to set the topic of.</param>
        /// <param name="topic">The new topic.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The channel's new topic.</returns>
        public async Task<string> SetTopic(string channelId, string topic, CancellationToken? cancellationToken = null) =>
            (await _client.Get<TopicResponse>("channels.setTopic", new Args
                {
                    { "channel", channelId },
                    { "topic", topic }
                }, cancellationToken).ConfigureAwait(false))
            .Topic;

        /// <summary>
        /// Unarchives a channel. The calling user is added to the channel.
        /// </summary>
        /// <param name="channelId">Channel to unarchive.</param>
        /// <param name="cancellationToken"></param>
        public Task Unarchive(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("channels.unarchive", new Args { { "channel", channelId } }, cancellationToken);
    }
}