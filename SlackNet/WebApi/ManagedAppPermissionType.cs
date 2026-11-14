namespace SlackNet.WebApi;

/// <summary>
/// Who can interact with a managed app.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.managed.permissions.set/">Slack documentation</a> for more information.</remarks>
public enum ManagedAppPermissionType
{
    /// <summary>
    /// All members can interact with the app.
    /// </summary>
    Everyone,

    /// <summary>
    /// Only the app's owner can interact with the app.
    /// </summary>
    AppOwner
}
