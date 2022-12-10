namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a Team Admin deletes field definitions from the team profile.
/// </summary>
public class TeamProfileDelete : Event
{
    public DeletedProfile Profile { get; set; }
}