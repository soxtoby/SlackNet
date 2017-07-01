using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a new team member joins the team.
    /// </summary>
    public class TeamJoin : Event
    {
        public User User { get; set; }
    }
}