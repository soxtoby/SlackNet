#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// The app config for a connector.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.config.lookup/">Slack documentation</a> for more information.</remarks>
public class AppConfig
{
    /// <summary>
    /// The app ID of the particular config.
    /// </summary>
    public required string AppId { get; set; }

    /// <summary>
    /// The workflow auth permission.
    /// </summary>
    public WorkflowAuthStrategy WorkflowAuthStrategy { get; set; }

    /// <summary>
    /// Domain restrictions for the app, if set.
    /// </summary>
    public AppDomainRestrictions DomainRestrictions { get; set; } = new();

    /// <summary>
    /// The app-level override for rich link preview. Will be null if not set.
    /// </summary>
    public RichLinkPreviewType? RichLinkPreviewType { get; set; }
}

/// <summary>
/// Domain restrictions for a connector.
/// If domain restrictions are set, users will only be able to connect their external accounts belonging to certain email domains or instance URLs.
/// </summary>
public class AppDomainRestrictions
{
    /// <summary>
    /// List of URLs for allowed connector authorization.
    /// </summary>
    public IList<string> Urls { get; set; } = [];

    /// <summary>
    /// List of emails for allowed connector authorization.
    /// </summary>
    public IList<string> Emails { get; set; } = [];
}

/// <summary>
/// Whether a workflow builder can delegate their auth for end-user workflow runs.
/// </summary>
public enum WorkflowAuthStrategy
{
    BuilderChoice,

    /// <summary>
    /// A workflow builder can't delegate their auth for end-user workflow runs.
    /// </summary>
    EndUserOnly
}

/// <summary>
/// An app-level override for rich link preview.
/// </summary>
public enum RichLinkPreviewType
{
    AllDetails,
    LimitedDetails,
    NoPreview,
    DefaultToGlobalSetting
}
