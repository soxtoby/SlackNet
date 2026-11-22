using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// A group of settings corresponding to the <b>Settings</b> section of an app's configuration pages.
/// </summary>
public class Settings
{
    /// <summary>
    /// IP addresses that conform to the <a href="https://docs.slack.dev/concepts/security/#verify">Allowed IP Ranges feature</a>.
    /// </summary>
    public IList<string>? AllowedIpAddressRanges { get; set; }

    /// <summary>
    /// Whether or not app-level token rotation is enabled.
    /// </summary>
    public bool? AppLevelTokenRotationEnabled { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/apis/events-api/">Events API</a> configuration for the app.
    /// </summary>
    public EventSubscriptions? EventSubscriptions { get; set; }

    /// <summary>
    /// Maps to <a href="https://docs.slack.dev/messaging/sending-messages-using-incoming-webhooks/#enable_webhooks">enabling incoming webhooks</a> via your app settings.
    /// </summary>
    public IncomingWebhooks? IncomingWebhooks { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/interactivity/">interactivity</a> configuration for the app.
    /// </summary>
    public Interactivity? Interactivity { get; set; }

    /// <summary>
    /// Whether or not <a href="https://docs.slack.dev/enterprise/organization-ready-apps/">organization-wide deployment</a> is enabled.
    /// Must be <c>true</c> if the manifest includes <see cref="AppManifest.Functions"/>, <see cref="AppManifest.Workflows"/>, <see cref="AppManifest.Datastores"/>,
    /// <see cref="AppManifest.Types"/>, <see cref="AppManifest.OutgoingDomains"/>, or <see cref="AppManifest.Events"/>.
    /// It must also be <c>true</c> if <see cref="FunctionRuntime"/> is <see cref="SlackNet.FunctionRuntime.Slack"/>, <see cref="IsHosted"/> is set,
    /// <see cref="OauthConfig.TokenManagementEnabled"/> is <c>false</c>, or <see cref="OauthConfig.IsEnterpriseManagedAuthSupported"/> is <c>true</c>.
    /// </summary>
    public bool? OrgDeployEnabled { get; set; }

    /// <summary>
    /// Whether or not <a href="https://docs.slack.dev/apis/events-api/using-socket-mode/">Socket Mode</a> is enabled.
    /// </summary>
    public bool? SocketModeEnabled { get; set; }

    /// <summary>
    /// Whether or not <a href="https://docs.slack.dev/authentication/using-token-rotation/">token rotation</a> is enabled.
    /// </summary>
    public bool? TokenRotationEnabled { get; set; }

    /// <summary>
    /// Whether the app is hosted by Slack.
    /// </summary>
    public bool? IsHosted { get; set; }

    /// <summary>
    /// Whether or not <a href="https://docs.slack.dev/ai/slack-mcp-server/developing/">MCP</a> is enabled for the app.
    /// </summary>
    public bool? IsMcpEnabled { get; set; }

    /// <summary>
    /// Indicates the use of SIWS Links.
    /// </summary>
    public SiwsLinks? SiwsLinks { get; set; }

    /// <summary>
    /// The runtime of any <see cref="AppManifest.Functions"/> declared in the manifest.
    /// </summary>
    public FunctionRuntime? FunctionRuntime { get; set; }
}

public enum FunctionRuntime
{
    /// <summary>
    /// For a self-hosted app.
    /// </summary>
    Remote,

    /// <summary>
    /// For an <a href="https://docs.slack.dev/workflows/">app built with the Deno Slack SDK</a>.
    /// </summary>
    Slack
}

/// <summary>
/// Settings that describe <a href="https://docs.slack.dev/apis/events-api/">Events API</a> configuration for the app.
/// </summary>
public class EventSubscriptions
{
    /// <summary>
    /// The full <c>https</c> URL that acts as the <a href="https://docs.slack.dev/apis/events-api/using-http-request-urls/#request-urls">Events API request URL</a>.
    /// If set, you'll need to manually verify the Request URL in the <b>App Manifest</b> section of your app's settings.
    /// </summary>
    public string? RequestUrl { get; set; }

    /// <summary>
    /// The <a href="https://docs.slack.dev/reference/events/">event types</a> you want the app to subscribe to.
    /// </summary>
    public IList<string>? BotEvents { get; set; }

    /// <summary>
    /// The <a href="https://docs.slack.dev/reference/events/">event types</a> you want the app to subscribe to on behalf of authorized users.
    /// </summary>
    public IList<string>? UserEvents { get; set; }

    /// <summary>
    /// Message metadata events to subscribe to.
    /// </summary>
    public IList<MetadataSubscription>? MetadataSubscriptions { get; set; }
}

/// <summary>
/// A subscription to message metadata events.
/// </summary>
public class MetadataSubscription
{
    public required string AppId { get; set; }
    public required string EventType { get; set; }
}

/// <summary>
/// Maps to <a href="https://docs.slack.dev/messaging/sending-messages-using-incoming-webhooks/#enable_webhooks">enabling incoming webhooks</a> via your app settings.
/// </summary>
public class IncomingWebhooks
{
    public bool? IncomingWebhooksEnabled { get; set; }
}

/// <summary>
/// Settings that describe <a href="https://docs.slack.dev/interactivity/">interactivity</a> configuration for the app.
/// </summary>
public class Interactivity
{
    /// <summary>
    /// Whether or not interactivity features are enabled.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The full <c>https</c> URL that acts as the <a href="https://docs.slack.dev/interactivity/handling-user-interaction/#setup">interactive <b>Request URL</b></a>.
    /// </summary>
    public string? RequestUrl { get; set; }

    /// <summary>
    /// The full <c>https</c> URL that acts as the <a href="https://docs.slack.dev/interactivity/handling-user-interaction/#setup">interactive <b>Options Load URL</b></a>.
    /// </summary>
    public string? MessageMenuOptionsUrl { get; set; }
}

/// <summary>
/// Indicates the use of SIWS Links.
/// </summary>
public class SiwsLinks
{
    public string? InitiateUri { get; set; }
}
