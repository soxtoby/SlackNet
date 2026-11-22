namespace SlackNet;

/// <summary>
/// A slash command an agent offers in a code channel.
/// When a user invokes it, Slack sends your app a <see cref="Interaction.SlashCommand"/> request.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/ai/slack-code/agent-slash-commands/">Slack documentation</a> for more information.</remarks>
public class AgentCommand
{
    /// <summary>
    /// The command name, without a leading slash (e.g. <c>create-pr</c>, not <c>/create-pr</c>).
    /// Must be unique within the agent's commands, and can't be a built-in Slack command.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Describes what the command does. Shown in the composer typeahead.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Hint for the command's arguments (e.g. <c>[title]</c>). Shown in the composer typeahead.
    /// </summary>
    public string? ArgumentHint { get; set; }

    /// <summary>
    /// Whether channel, user, and link references in the command's <see cref="Interaction.SlashCommand.Text"/> are escaped before delivery.
    /// </summary>
    public bool ShouldEscape { get; set; }
}
