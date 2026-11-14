#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Configures connectors in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAppsConfigApi
{
    /// <summary>
    /// Look up the app config for connectors by their IDs.
    /// If domain restrictions (email and instance URL) are set, users will only be able to connect their external accounts belonging to certain email domains or instance URLs.
    /// If builder auth restrictions are set, a workflow builder will not be able to delegate their auth for end-user workflow runs.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.config.lookup/">Slack documentation</a> for more information.</remarks>
    /// <param name="appIds">App IDs to get app configs for.</param>
    /// <param name="richLinkPreviewTypes">Return apps with the corresponding rich link preview layouts.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of app configs.</returns>
    Task<IList<AppConfig>> Lookup(
        IEnumerable<string>? appIds = null,
        IEnumerable<RichLinkPreviewType>? richLinkPreviewTypes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the app config for a connector.
    /// Sets domain restrictions (email and instance URL) and auth permissions.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.config.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">The encoded app ID to set the app config for.</param>
    /// <param name="workflowAuthStrategy">The workflow auth permission.</param>
    /// <param name="richLinkPreviewType">Indicates the app-level override for rich link preview. Unsupported for free teams.</param>
    /// <param name="domainRestrictions">Domain restrictions for the app, which set the allowed URLs and emails for connector authorization.</param>
    /// <param name="cancellationToken"></param>
    Task Set(
        string appId,
        WorkflowAuthStrategy? workflowAuthStrategy = null,
        RichLinkPreviewType? richLinkPreviewType = null,
        AppDomainRestrictions? domainRestrictions = null,
        CancellationToken cancellationToken = default);
}

public class AdminAppsConfigApi(ISlackApiClient client) : IAdminAppsConfigApi
{
    public async Task<IList<AppConfig>> Lookup(
        IEnumerable<string>? appIds = null,
        IEnumerable<RichLinkPreviewType>? richLinkPreviewTypes = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<AppConfigLookupResponse>("admin.apps.config.lookup", new Args
            {
                { "app_ids", appIds },
                { "rich_link_preview_types", richLinkPreviewTypes }
            }, cancellationToken).ConfigureAwait(false)).Configs;

    public Task Set(
        string appId,
        WorkflowAuthStrategy? workflowAuthStrategy = null,
        RichLinkPreviewType? richLinkPreviewType = null,
        AppDomainRestrictions? domainRestrictions = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.apps.config.set", new Args
            {
                { "app_id", appId },
                { "workflow_auth_strategy", workflowAuthStrategy },
                { "rich_link_preview_type", richLinkPreviewType },
                { "domain_restrictions", domainRestrictions }
            }, cancellationToken);
}
