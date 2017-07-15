using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class BotsApi
    {
        private readonly SlackApiClient _client;
        public BotsApi(SlackApiClient client) => _client = client;

        /// <summary>
        /// Returns information about a bot user.
        /// <see cref="botId"/> is returned from <see cref="BotMessage"/> events and in the response of methods like <see cref="ChannelsApi.History"/>.
        /// </summary>
        /// <param name="botId">Bot user to get info on.</param>
        /// <param name="cancellationToken"></param>
        public async Task<BotInfo> Info(string botId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<BotsInfoResponse>("bots.info", new Args { { "bot", botId } }, cancellationToken).ConfigureAwait(false)).Bot;
    }
}