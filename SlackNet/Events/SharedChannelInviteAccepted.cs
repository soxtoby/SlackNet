using System.Collections.Generic;

namespace SlackNet.Events
{
    public class SharedChannelInviteAccepted : Event
    {
        public bool ApprovalRequired { get; set; }
        public Invite Invite { get; set; }
        public User InvitingUser { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientUserId { get; set; }
        public InviteChannel Channel { get; set; }
        public IList<ConnectedTeam> TeamsInChannel { get; set; } = new List<ConnectedTeam>();
        public User AcceptingUser { get; set; }
    }
}