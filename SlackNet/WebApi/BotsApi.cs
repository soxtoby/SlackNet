using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IBotsApi
{
    /// <summary>
    /// Returns information about a bot user.
    /// <paramref name="botId"/> is returned from <see cref="BotMessage"/> events and in the response of methods like <see cref="ConversationsApi.History"/>.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/bots.info">Slack documentation</a> for more information.</remarks>
    /// <param name="botId">Bot user to get info on.</param>
    /// <param name="cancellationToken"></param>
    Task<BotInfo> Info(string botId, CancellationToken? cancellationToken = null);
}

public class BotsApi : IBotsApi
{
    private readonly ISlackApiClient _client;
    public BotsApi(ISlackApiClient client) => _client = client;

    public async Task<BotInfo> Info(string botId, CancellationToken? cancellationToken = null) =>
        (await _client.Get<BotsInfoResponse>("bots.info", new Args { { "bot", botId } }, cancellationToken).ConfigureAwait(false)).Bot;
}