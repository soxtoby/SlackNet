using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface ITeamProfileApi
    {
        /// <summary>
        /// Used to get the profile field definitions for this team.
        /// </summary>
        /// <remarks>See the <a href="https://api.slack.com/methods/team.profile.get">Slack documentation</a> for more information.</remarks>
        /// <param name="visibility">Filter by visibility.</param>
        /// <param name="cancellationToken"></param>
        Task<TeamProfile> Get(ProfileFieldVisibility visibility = ProfileFieldVisibility.All, CancellationToken? cancellationToken = null);
    }

    public class TeamProfileApi : ITeamProfileApi
    {
        private readonly ISlackApiClient _client;
        public TeamProfileApi(ISlackApiClient client) => _client = client;

        public async Task<TeamProfile> Get(ProfileFieldVisibility visibility = ProfileFieldVisibility.All, CancellationToken? cancellationToken = null) =>
            (await _client.Get<TeamProfileResponse>("team.profile.get", new Args { { "visibility", visibility } }, cancellationToken).ConfigureAwait(false)).Profile;
    }
}