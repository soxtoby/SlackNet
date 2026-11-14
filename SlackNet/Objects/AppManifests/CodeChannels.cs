#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe <a href="https://docs.slack.dev/ai/slack-code/">code channels</a> configuration.
/// </summary>
public class CodeChannels
{
    /// <summary>
    /// Whether or not code channels are enabled for the app.
    /// When <c>true</c>, your app receives <see cref="Events.AppMention"/> events and can call the <see cref="WebApi.IAgentsConversationsApi"/> methods.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// The <c>https</c> URL where Slack delivers invocations of your agent's
    /// <a href="https://docs.slack.dev/ai/slack-code/agent-slash-commands/">runtime-registered slash commands</a>.
    /// Not needed for Socket Mode or hosted apps.
    /// </summary>
    public string? SlashCommandUrl { get; set; }

    /// <summary>
    /// Whether or not Slack skips showing the user an ephemeral prompt when they mention your app.
    /// When <c>true</c>, your app receives <see cref="Events.AppMention"/> events directly and decides whether to reply in place or create a code channel.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool? SuppressMentionEphemeral { get; set; }
}
