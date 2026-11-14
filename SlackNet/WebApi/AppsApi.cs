#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IAppsApi
{
    /// <summary>
    /// Uninstalls your app from a workspace.
    /// Unlike <see cref="IAuthApi.Revoke"/>, which revokes a single token, this method revokes <i>all</i> tokens associated with a single installation of an app.
    /// </summary>
    /// <remarks>
    /// Called with a token from the installation being uninstalled, which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/apps.uninstall/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="clientId">Issued when you created your application.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="cancellationToken"></param>
    Task Uninstall(string clientId, string clientSecret, CancellationToken cancellationToken = default);
}

public class AppsApi(ISlackApiClient client) : IAppsApi
{
    public Task Uninstall(string clientId, string clientSecret, CancellationToken cancellationToken = default) =>
        client.Post("apps.uninstall", new Args(), new SlackFormContent // apps.uninstall doesn't accept JSON, and form content keeps the client secret out of the URL
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret }
                },
            cancellationToken);
}
