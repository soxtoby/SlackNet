#nullable enable
using System.Collections.Generic;
using SlackNet.WebApi;

namespace SlackNet.AppManifests;

/// <summary>
/// Settings related to assistant view for <a href="https://docs.slack.dev/ai/">apps using AI features</a>.
/// </summary>
public class AssistantView
{
    /// <summary>
    /// Actions available in the assistant container.
    /// </summary>
    public IList<AgentAction>? Actions { get; set; }

    /// <summary>
    /// A description of the app assistant.
    /// </summary>
    public required string AssistantDescription { get; set; }

    /// <summary>
    /// Hard-coded prompts for the app assistant container to prompt a user.
    /// </summary>
    public IList<AssistantPrompt>? SuggestedPrompts { get; set; }
}
