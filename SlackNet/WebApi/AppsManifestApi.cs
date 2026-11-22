using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet.AppManifests;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Creates and configures apps from app manifests.
/// These methods require an <a href="https://docs.slack.dev/authentication/tokens/#config">app configuration access token</a>,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAppsManifestApi
{
    /// <summary>
    /// Create an app from an app manifest.
    /// </summary>
    /// <remarks>
    /// If the manifest doesn't match the <a href="https://docs.slack.dev/reference/app-manifest/">correct schema</a>,
    /// a <see cref="SlackException"/> is thrown with an <c>invalid_manifest</c> error,
    /// and its <see cref="SlackException.ErrorDetails"/> describe each problem and its location within the manifest.
    /// See the <a href="https://docs.slack.dev/reference/methods/apps.manifest.create/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="manifest">The app manifest.</param>
    /// <param name="teamId">When called with an org token, which specific team to create the app on.</param>
    /// <param name="cancellationToken"></param>
    Task<AppCreationResponse> Create(AppManifest manifest, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes an app created through app manifests.
    /// When called with a manager app token, this method can only delete apps that were created by that manager app.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.manifest.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">The ID of the app you want to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string appId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Export an app manifest from an existing app.
    /// When called with a manager app token, this method can only export apps that were created by that manager app.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.manifest.export/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">The ID of the app whose configuration you want to export as a manifest.</param>
    /// <param name="cancellationToken"></param>
    Task<AppManifest> Export(string appId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an app from an app manifest.
    /// When called with a manager app token, this method can only update apps that were created by that manager app.
    /// </summary>
    /// <remarks>
    /// If the manifest doesn't match the <a href="https://docs.slack.dev/reference/app-manifest/">correct schema</a>,
    /// a <see cref="SlackException"/> is thrown with an <c>invalid_manifest</c> error,
    /// and its <see cref="SlackException.ErrorDetails"/> describe each problem and its location within the manifest.
    /// See the <a href="https://docs.slack.dev/reference/methods/apps.manifest.update/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="appId">The ID of the app whose configuration you want to update.</param>
    /// <param name="manifest">
    /// The app manifest. As this method entirely <i>replaces</i> any previous configuration, the manifest must contain both unmodified and modified fields.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>Whether the app's permissions were updated.</returns>
    Task<bool> Update(string appId, AppManifest manifest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate an app manifest.
    /// </summary>
    /// <remarks>
    /// If the manifest doesn't match the <a href="https://docs.slack.dev/reference/app-manifest/">correct schema</a>,
    /// a <see cref="SlackException"/> is thrown with an <c>invalid_manifest</c> error,
    /// and its <see cref="SlackException.ErrorDetails"/> describe each problem and its location within the manifest.
    /// See the <a href="https://docs.slack.dev/reference/methods/apps.manifest.validate/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="manifest">The manifest to be validated.</param>
    /// <param name="appId">
    /// The ID of the app whose configuration you want to validate.
    /// When called with a manager app token, this method can only validate against apps that were created by that manager app.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>Non-blocking warnings from app manifest validation.</returns>
    Task<IList<ErrorDetail>> Validate(AppManifest manifest, string? appId = null, CancellationToken cancellationToken = default);
}

public class AppsManifestApi(ISlackApiClient client, SlackJsonSettings jsonSettings) : IAppsManifestApi
{
    public Task<AppCreationResponse> Create(AppManifest manifest, string? teamId = null, CancellationToken cancellationToken = default) =>
        client.Post<AppCreationResponse>("apps.manifest.create", new Args
            {
                { "manifest", Serialize(manifest) },
                { "team_id", teamId }
            }, cancellationToken);

    public Task Delete(string appId, CancellationToken cancellationToken = default) =>
        client.Post("apps.manifest.delete", new Args { { "app_id", appId } }, cancellationToken);

    public async Task<AppManifest> Export(string appId, CancellationToken cancellationToken = default) =>
        (await client.Post<AppManifestResponse>("apps.manifest.export", new Args { { "app_id", appId } }, cancellationToken).ConfigureAwait(false)).Manifest;

    public async Task<bool> Update(string appId, AppManifest manifest, CancellationToken cancellationToken = default) =>
        (await client.Post<AppManifestUpdateResponse>("apps.manifest.update", new Args
            {
                { "app_id", appId },
                { "manifest", Serialize(manifest) }
            }, cancellationToken).ConfigureAwait(false)).PermissionsUpdated;

    public async Task<IList<ErrorDetail>> Validate(AppManifest manifest, string? appId = null, CancellationToken cancellationToken = default) =>
        (await client.Post<ManifestValidationResponse>("apps.manifest.validate", new Args
            {
                { "manifest", Serialize(manifest) },
                { "app_id", appId }
            }, cancellationToken).ConfigureAwait(false)).Warnings;

    /// <summary>
    /// Slack expects the manifest as a JSON string, rather than a JSON object.
    /// </summary>
    private string Serialize(AppManifest manifest) =>
        JsonConvert.SerializeObject(manifest, jsonSettings.SerializerSettings);
}
