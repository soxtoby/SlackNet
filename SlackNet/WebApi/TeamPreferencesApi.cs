using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface ITeamPreferencesApi
{
    /// <summary>
    /// Retrieve a list of a workspace's team preferences.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/team.preferences.list">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<TeamPreferences> List(CancellationToken? cancellationToken = null);
}

public class TeamPreferencesApi : ITeamPreferencesApi
{
    private readonly ISlackApiClient _client;
    public TeamPreferencesApi(ISlackApiClient client) => _client = client;

    public Task<TeamPreferences> List(CancellationToken? cancellationToken = null) => 
        _client.Get<TeamPreferences>("team.preferences.list", new Args(), cancellationToken);
}