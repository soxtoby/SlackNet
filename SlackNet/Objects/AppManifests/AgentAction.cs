namespace SlackNet.AppManifests;

/// <summary>
/// An action available in the agent or assistant container.
/// </summary>
public class AgentAction
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}
