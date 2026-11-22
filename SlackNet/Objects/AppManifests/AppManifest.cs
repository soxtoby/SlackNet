using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.AppManifests;

/// <summary>
/// An app's configuration.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/app-manifest/">Slack documentation</a> for more information.</remarks>
public class AppManifest
{
    /// <summary>
    /// A group of settings that describe the manifest.
    /// </summary>
    [JsonProperty("_metadata")]
    public Metadata? Metadata { get; set; }

    /// <summary>
    /// Information to be listed in the Slack Marketplace.
    /// Only relevant for apps that intend to be distributed via the Slack Marketplace.
    /// </summary>
    public AppDirectory? AppDirectory { get; set; }

    /// <summary>
    /// A group of settings that describe parts of an app's appearance within Slack.
    /// </summary>
    public required DisplayInformation DisplayInformation { get; set; }

    /// <summary>
    /// A group of settings corresponding to the <b>Features</b> section of an app's configuration pages.
    /// </summary>
    public Features? Features { get; set; }

    /// <summary>
    /// A group of settings describing OAuth configuration for the app.
    /// </summary>
    public OauthConfig? OauthConfig { get; set; }

    /// <summary>
    /// A group of settings corresponding to the <b>Settings</b> section of an app's configuration pages.
    /// </summary>
    public Settings? Settings { get; set; }

    /// <summary>
    /// Custom workflow steps available for use in workflows either defined in the manifest or built directly in Workflow Builder,
    /// keyed by the <c>callback_id</c> of the step.
    /// </summary>
    public IDictionary<string, JsonData>? Functions { get; set; }

    /// <summary>
    /// The workflows the app provides, keyed by callback ID.
    /// </summary>
    public IDictionary<string, JsonData>? Workflows { get; set; }

    /// <summary>
    /// The datastores used by the app, keyed by name.
    /// </summary>
    public IDictionary<string, JsonData>? Datastores { get; set; }

    /// <summary>
    /// Accepted egress domains for an app with a <see cref="Settings.FunctionRuntime"/> of <see cref="FunctionRuntime.Slack"/>.
    /// </summary>
    public IList<string>? OutgoingDomains { get; set; }

    /// <summary>
    /// The types the app provides, keyed by type name.
    /// </summary>
    public IDictionary<string, JsonData>? Types { get; set; }

    /// <summary>
    /// The message metadata events the app can send, keyed by <c>event_type</c>.
    /// When the app sends message metadata with a declared <c>event_type</c>, Slack validates the <c>event_payload</c> against it.
    /// </summary>
    public IDictionary<string, JsonData>? Events { get; set; }

    /// <summary>
    /// The OAuth configuration used by the app.
    /// </summary>
    public JsonData? ExternalAuthProviders { get; set; }

    /// <summary>
    /// Compliance certifications for GovSlack.
    /// </summary>
    public Compliance? Compliance { get; set; }

    /// <summary>
    /// The MCP servers used by the app, keyed by server key.
    /// </summary>
    public IDictionary<string, McpServer>? McpServers { get; set; }
}
