using System.Collections.Generic;

namespace SlackNet.Events;

public class SharedChannelInviteDeclined : Event
{
    public Invite Invite { get; set; }
    public InviteChannel Channel { get; set; }
    public string DecliningTeamId { get; set; }
    public IList<ConnectedTeam> TeamsInChannel { get; set; } = [];
    public User DecliningUser { get; set; }
}