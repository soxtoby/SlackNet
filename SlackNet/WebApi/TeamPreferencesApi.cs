using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface ITeamPreferencesApi
{
    /// <summary>
    /// Retrieve a list of a workspace's team preferences.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/team.preferences.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<TeamPreferences> List(CancellationToken cancellationToken = default);
}

public class TeamPreferencesApi(ISlackApiClient client) : ITeamPreferencesApi
{
    public Task<TeamPreferences> List(CancellationToken cancellationToken = default) => 
        client.Get<TeamPreferences>("team.preferences.list", new Args(), cancellationToken);
}