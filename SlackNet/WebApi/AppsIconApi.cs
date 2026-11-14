#nullable enable
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Sets app icons.
/// These methods require an <a href="https://docs.slack.dev/authentication/tokens/#config">app configuration access token</a>,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// When called with a manager app token, these methods can only set icons for apps that were created by that manager app.
/// </summary>
public interface IAppsIconApi
{
    /// <summary>
    /// Sets the app icon from an uploaded image.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.icon.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">The ID of the app whose icon you want to set.</param>
    /// <param name="image">The image file. Only its <see cref="FileUpload.FileName"/> and content are used.</param>
    /// <param name="cancellationToken"></param>
    Task Set(string appId, FileUpload image, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the app icon from a publicly hosted image.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.icon.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">The ID of the app whose icon you want to set.</param>
    /// <param name="url">URL of a publicly hosted image.</param>
    /// <param name="cancellationToken"></param>
    Task Set(string appId, string url, CancellationToken cancellationToken = default);
}

public class AppsIconApi(ISlackApiClient client) : IAppsIconApi
{
    public async Task Set(string appId, FileUpload image, CancellationToken cancellationToken = default)
    {
        using var content = image.GetContent();
        await client.Post("apps.icon.set", new Args { { "app_id", appId } }, new MultipartFormDataContent
                {
                    { content.HttpContent, "file", image.FileName }
                },
            cancellationToken).ConfigureAwait(false);
    }

    public Task Set(string appId, string url, CancellationToken cancellationToken = default) =>
        client.Post("apps.icon.set", new Args
            {
                { "app_id", appId },
                { "url", url }
            }, cancellationToken);
}
