using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IMigrationApi
{
    /// <summary>
    /// For Enterprise Grid workspaces, map local user IDs to global user IDs.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/migration.exchange">Slack documentation</a> for more information.</remarks>
    /// <param name="userIds">List of user ids, up to 400 per request.</param>
    /// <param name="toOld">Specify true to convert W global user IDs to workspace-specific U IDs. Defaults to False.</param>
    /// <param name="cancellationToken"></param>
    Task<MigrationResponse> Exchange(IEnumerable<string> userIds, bool toOld = false, CancellationToken cancellationToken = default);
}

public class MigrationApi(ISlackApiClient client) : IMigrationApi
{
    public Task<MigrationResponse> Exchange(IEnumerable<string> userIds, bool toOld = false, CancellationToken cancellationToken = default) =>
        client.Get<MigrationResponse>("migration.exchange", new Args
            {
                { "users", userIds },
                { "to_old", toOld }
            }, cancellationToken);
}