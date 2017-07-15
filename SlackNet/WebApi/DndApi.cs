using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class DndApi
    {
        private readonly SlackApiClient _client;
        public DndApi(SlackApiClient client) => _client = client;

        /// <summary>
        /// Ends the user's currently scheduled Do Not Disturb session immediately.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task EndDnd(CancellationToken? cancellationToken = null) =>
            _client.Get("dnd.endDnd", new Args(), cancellationToken);

        /// <summary>
        /// Ends the current user's snooze mode immediately.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<DndResponse> EndSnooze(CancellationToken? cancellationToken = null) =>
            _client.Get<DndResponse>("dnd.endSnooze", new Args(), cancellationToken);

        /// <summary>
        /// Provides information about the current user's Do Not Disturb settings.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<OwnDndResponse> Info(CancellationToken? cancellationToken = null) =>
            _client.Get<OwnDndResponse>("dnd.info", new Args(), cancellationToken);

        /// <summary>
        /// Provides information about a user's current Do Not Disturb settings.
        /// </summary>
        /// <param name="userId">User to fetch status for.</param>
        /// <param name="cancellationToken"></param>
        public Task<DndResponse> Info(string userId, CancellationToken? cancellationToken = null) =>
            _client.Get<DndResponse>("dnd.info", new Args { { "user", userId } }, cancellationToken);

        /// <summary>
        /// Adjusts the snooze duration for a user's Do Not Disturb settings.
        /// If a snooze session is not already active for the user, invoking this method will begin one for the specified duration.
        /// </summary>
        /// <param name="numMinutes">Number of minutes, from now, to snooze until.</param>
        /// <param name="cancellationToken"></param>
        public Task<SnoozeResponse> SetSnooze(int numMinutes, CancellationToken? cancellationToken) =>
            _client.Get<SnoozeResponse>("dnd.setSnooze", new Args { { "num_minutes", numMinutes } }, cancellationToken);

        /// <summary>
        /// Provides information about the current Do Not Disturb settings for users of a Slack team.
        /// </summary>
        /// <param name="userIds">List of users to fetch Do Not Disturb status for (defaults to entire team).</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Mapping of user to DnD status.</returns>
        public async Task<IReadOnlyDictionary<string, DndStatus>> TeamInfo(IEnumerable<string> userIds = null, CancellationToken? cancellationToken = null) =>
            (await _client.Get<TeamDndResponse>("dnd.teamInfo", new Args { { "users", userIds } }, cancellationToken).ConfigureAwait(false)).Users;
    }
}