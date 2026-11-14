#nullable enable
using System.Collections.Generic;
using SlackNet.WebApi;

namespace SlackNet.AppManifests;

/// <summary>
/// Settings related to the agent view for <a href="https://docs.slack.dev/ai/agents/">apps using AI features</a>.
/// </summary>
public class AgentView
{
    /// <summary>
    /// Actions available in the agent container.
    /// </summary>
    public IList<AgentAction>? Actions { get; set; }

    /// <summary>
    /// A description of the agent.
    /// </summary>
    public required string AgentDescription { get; set; }

    /// <summary>
    /// Hard-coded prompts for the agent container to prompt a user.
    /// </summary>
    public IList<AssistantPrompt>? SuggestedPrompts { get; set; }
}
