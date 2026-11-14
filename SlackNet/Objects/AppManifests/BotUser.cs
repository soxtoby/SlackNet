#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe bot user configuration.
/// </summary>
public class BotUser
{
    /// <summary>
    /// The display name of the bot user.
    /// Slack also generates the bot's username from this value: it's lowercased, and characters other than <c>a-z</c>, <c>0-9</c>, <c>-</c>, <c>_</c>, and <c>.</c> are replaced.
    /// The display name is rejected if the resulting username is reserved.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Whether or not the bot user will always appear to be online.
    /// </summary>
    public bool? AlwaysOnline { get; set; }
}
