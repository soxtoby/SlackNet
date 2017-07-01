using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events.Messages;
using SlackNet.Objects;
using SlackNet.WebApi.Responses;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class ImApi
    {
        private readonly WebApiClient _client;
        public ImApi(WebApiClient client) => _client = client;

        /// <summary>
        /// Closes a direct message channel.
        /// </summary>
        /// <param name="channelId">Direct message channel to close.</param>
        /// <param name="cancellationToken"></param>
        public Task Close(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("im.close", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Returns a portion of message events from the specified direct message channel.
        /// To read the entire history for a direct message channel, call the method with no latest or oldest arguments, and then continue paging.
        /// To retrieve a single message, specify its ts value as latest, set inclusive to true, and dial your count down to 1.
        /// See https://api.slack.com/methods/groups.history for more information.
        /// </summary>
        /// <param name="channelId">Direct message channel to fetch history for.</param>
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
            _client.Get<ChannelHistoryResponse>("im.history", new Args
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
        /// Returns a list of all im channels that the user has.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Im>> List(CancellationToken? cancellationToken = null) =>
            (await _client.Get<ImListResponse>("im.list", new Args(), cancellationToken).ConfigureAwait(false)).Ims;

        /// <summary>
        /// Moves the read cursor in a direct message channel.
        /// </summary>
        /// <param name="channelId">Direct message channel to set reading cursor in.</param>
        /// <param name="ts">Timestamp of the most recently seen message.</param>
        /// <param name="cancellationToken"></param>
        public Task Mark(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("im.mark", new Args
                {
                    { "channel", channelId },
                    { "ts", ts }
                }, cancellationToken);

        /// <summary>
        /// Opens a direct message channel with another member of your Slack team.
        /// </summary>
        /// <param name="userId">User to open a direct message channel with.</param>
        /// <param name="returnIm">Indicates you want the full IM channel definition in the response.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Im> Open(string userId, bool returnIm = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ImResponse>("im.open", new Args
                {
                    { "user", userId },
                    { "return_im", returnIm }
                }, cancellationToken).ConfigureAwait(false))
            .Channel;

        /// <summary>
        /// Returns an entire thread (a message plus all the messages in reply to it).
        /// </summary>
        /// <param name="channelId">Direct message channel to fetch thread from.</param>
        /// <param name="threadTs">Unique identifier of a thread's parent message.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Message>> Replies(string channelId, string threadTs, CancellationToken? cancellationToken = null) =>
            (await _client.Get<RepliesResponse>("im.replies", new Args
                {
                    { "channel", channelId },
                    { "thread_ts", threadTs }
                }, cancellationToken).ConfigureAwait(false))
            .Messages;
    }
}