namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe a <a href="https://docs.slack.dev/interactivity/implementing-slash-commands/">slash command</a>.
/// </summary>
public class SlashCommand
{
    /// <summary>
    /// The actual slash command. Should include the leading <c>/</c> character.
    /// </summary>
    public required string Command { get; set; }

    /// <summary>
    /// A description of the slash command that will be displayed to users.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Whether or not channels, users, and links typed with the slash command should be escaped. Defaults to <c>false</c>.
    /// </summary>
    public bool? ShouldEscape { get; set; }

    /// <summary>
    /// The full <c>https</c> URL that acts as the slash command's <a href="https://docs.slack.dev/interactivity/implementing-slash-commands/#creating_commands">request URL</a>.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// A hint about how to use the slash command for users.
    /// </summary>
    public string? UsageHint { get; set; }
}
