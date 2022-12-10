using System.Collections.Generic;

namespace SlackNet.Events;

public class SharedChannelInviteApproved : Event
{
    public Invite Invite { get; set; }
    public InviteChannel Channel { get; set; }
    public string ApprovingTeamId { get; set; }
    public IList<ConnectedTeam> TeamsInChannel { get; set; } = new List<ConnectedTeam>();
    public User ApprovingUser { get; set; }
}