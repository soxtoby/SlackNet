using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IMigrationApi
    {
        /// <summary>
        /// For Enterprise Grid workspaces, map local user IDs to global user IDs.
        /// </summary>
        /// <param name="userIds">List of user ids, up to 400 per request.</param>
        /// <param name="toOld">Specify true to convert W global user IDs to workspace-specific U IDs. Defaults to False.</param>
        /// <param name="cancellationToken"></param>
        Task<MigrationResponse> Exchange(IEnumerable<string> userIds, bool toOld = false, CancellationToken? cancellationToken = null);
    }

    public class MigrationApi : IMigrationApi
    {
        private readonly ISlackApiClient _client;
        public MigrationApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// For Enterprise Grid workspaces, map local user IDs to global user IDs.
        /// </summary>
        /// <param name="userIds">List of user ids, up to 400 per request.</param>
        /// <param name="toOld">Specify true to convert W global user IDs to workspace-specific U IDs. Defaults to False.</param>
        /// <param name="cancellationToken"></param>
        public Task<MigrationResponse> Exchange(IEnumerable<string> userIds, bool toOld = false, CancellationToken? cancellationToken = null) =>
            _client.Get<MigrationResponse>("migration.exchange", new Args
                {
                    { "users", userIds },
                    { "to_old", toOld }
                }, cancellationToken);
    }
}