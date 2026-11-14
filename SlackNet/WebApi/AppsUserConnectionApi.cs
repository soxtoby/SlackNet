#nullable enable
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IAppsUserConnectionApi
{
    /// <summary>
    /// Updates the connection status between a user and an app.
    /// Use this to complete a user's request to connect with or disconnect from your app, received as a <see cref="UserConnection"/> event.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.user.connection.update/">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">The ID of the user for the status update.</param>
    /// <param name="status">The status that should be set for the user.</param>
    /// <param name="cancellationToken"></param>
    Task Update(string userId, UserConnectionStatus status, CancellationToken cancellationToken = default);
}

public class AppsUserConnectionApi(ISlackApiClient client) : IAppsUserConnectionApi
{
    public Task Update(string userId, UserConnectionStatus status, CancellationToken cancellationToken = default) =>
        client.Post("apps.user.connection.update", new Args
            {
                { "user_id", userId },
                { "status", status }
            }, cancellationToken);
}
