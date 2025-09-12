using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface ITeamBillingApi
{
    /// <summary>
    /// Reads a workspace's billing plan information.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/team.billing.info/">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<BillingInfo> Info(CancellationToken cancellationToken = default);
}

public class TeamBillingApi(ISlackApiClient client) : ITeamBillingApi
{
    public Task<BillingInfo> Info(CancellationToken cancellationToken = default) => 
        client.Get<BillingInfo>("team.billing.info", new Args(), cancellationToken);
}