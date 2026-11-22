using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Configures the permissions of managed apps.
/// These methods require a user token or bot token issued to the manager app, which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// Either way, these methods can only set permissions for apps that were created by that manager app.
/// </summary>
public interface IAppsManagedPermissionsApi
{
    /// <summary>
    /// Set who can interact with a managed app.
    /// Lets the builder who created a managed app on a partner platform configure the app's permissions, using a user token or bot token from the manager app.
    /// Permissions can only be set before the app is installed: if the app is already installed, this method makes no change and returns the app's current permissions.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.managed.permissions.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">ID of the managed app to configure.</param>
    /// <param name="permissions">Who can interact with the app.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The app's permissions, which are unchanged if the app was already installed.</returns>
    Task<AppPermissions> Set(string appId, ManagedAppPermissionType permissions, CancellationToken cancellationToken = default);
}

public class AppsManagedPermissionsApi(ISlackApiClient client) : IAppsManagedPermissionsApi
{
    public Task<AppPermissions> Set(string appId, ManagedAppPermissionType permissions, CancellationToken cancellationToken = default) =>
        client.Post<AppPermissions>("apps.managed.permissions.set", new Args
            {
                { "app_id", appId },
                { "permissions", permissions }
            }, cancellationToken);
}
