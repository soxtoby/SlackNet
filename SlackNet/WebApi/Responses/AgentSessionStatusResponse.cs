namespace SlackNet.WebApi;

public class AgentSessionStatusResponse : SlackObject
{
    /// <summary>
    /// The status of the session as a whole, derived from the statuses of all the agents in it
    /// (<see cref="AgentSessionStatus.Suspended"/> over <see cref="AgentSessionStatus.Processing"/> over <see cref="AgentSessionStatus.Active"/> over <see cref="AgentSessionStatus.Closed"/>).
    /// </summary>
    public AgentSessionStatus Status { get; set; }

    /// <summary>
    /// The calling agent's own status, i.e. the value that was just written.
    /// In a session with more than one agent this can differ from <see cref="Status"/>.
    /// </summary>
    public AgentSessionStatus AgentStatus { get; set; }

    /// <summary>
    /// The title of the session, if it has one.
    /// </summary>
    public string Title { get; set; }
}