using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events.Messages;
using SlackNet.Objects;
using SlackNet.WebApi.Responses;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class BotsApi
    {
        private readonly WebApiClient _client;
        public BotsApi(WebApiClient client) => _client = client;

        /// <summary>
        /// Returns information about a bot user.
        /// <see cref="botId"/> is returned from <see cref="BotMessage"/> events and in the response of methods like <see cref="ChannelsApi.History"/>.
        /// </summary>
        /// <param name="botId">Bot user to get info on.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Bot> Info(string botId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<BotsInfoResponse>("bots.info", new Args { { "bot", botId } }, cancellationToken).ConfigureAwait(false)).Bot;
    }
}