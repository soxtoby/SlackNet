using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class MpimApi
    {
        private readonly SlackApiClient _client;
        public MpimApi(SlackApiClient client) => _client = client;

        /// <summary>
        /// Closes a multiparty direct message channel.
        /// </summary>
        /// <param name="channelId">MPIM to close.</param>
        /// <param name="cancellationToken"></param>
        public Task Close(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("mpim.close", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Returns a portion of message events from the specified multiparty direct message channel.
        /// To read the entire history for a multiparty direct message channel, call the method with no latest or oldest arguments, and then continue paging.
        /// To retrieve a single message, specify its ts value as latest, set inclusive to true, and dial your count down to 1.
        /// See https://api.slack.com/methods/groups.history for more information.
        /// </summary>
        /// <param name="channelId">Multiparty direct message channel to fetch history for.</param>
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
            _client.Get<ChannelHistoryResponse>("mpim.history", new Args
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
        /// Returns a list of all multiparty direct message channels that the user has.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Channel>> List(CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupListResponse>("mpim.list", new Args(), cancellationToken).ConfigureAwait(false)).Groups;

        /// <summary>
        /// Moves the read cursor in a multiparty direct message channel.
        /// </summary>
        /// <param name="channelId">Multiparty direct message channel to set reading cursor in.</param>
        /// <param name="ts">Timestamp of the most recently seen message.</param>
        /// <param name="cancellationToken"></param>
        public Task Mark(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("mpim.mark", new Args
                {
                    { "channel", channelId },
                    { "ts", ts }
                }, cancellationToken);

        /// <summary>
        /// Opens a multiparty direct message channel with up to 8 members of your Slack team.
        /// </summary>
        /// <param name="userIds">List of users. The ordering of the users is preserved whenever a MPIM group is returned.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Channel> Open(IEnumerable<string> userIds, CancellationToken? cancellationToken = null) =>
            (await _client.Get<GroupResponse>("mpim.open", new Args { { "users", userIds } }, cancellationToken).ConfigureAwait(false)).Group;

        /// <summary>
        /// Returns an entire thread (a message plus all the messages in reply to it).
        /// </summary>
        /// <param name="channelId">Multiparty direct message channel to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<MessageEvent>> Replies(string channelId, string threadTs, CancellationToken? cancellationToken = null) =>
            (await _client.Get<RepliesResponse>("mpim.replies", new Args
                {
                    { "channel", channelId },
                    { "thread_ts", threadTs }
                }, cancellationToken).ConfigureAwait(false))
            .Messages;
    }
}