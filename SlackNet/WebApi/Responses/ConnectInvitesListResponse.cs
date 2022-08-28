using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class ConnectInvitesListResponse
    {
        public IList<PendingInvite> Invites { get; set; }
        public ResponseMetadata ResponseMetadata { get; set; } = new();
    }

    public class PendingInvite
    {
        public string Direction { get; set; }
        public string Status { get; set; }
        public int DateLastUpdated { get; set; }
        public string InviteType { get; set; }
        public Invite Invite { get; set; }
        public InviteChannel Channel { get; set; }
        public IList<InviteAcceptance> Acceptances { get; set; } = new List<InviteAcceptance>();
    }

    public class InviteAcceptance
    {
        public string ApprovalStatus { get; set; }
        public int DateAccepted { get; set; }
        public int DateInvalid { get; set; }
        public int DateLastUpdated { get; set; }
        public ConnectedTeam AcceptingTeam { get; set; }
        public User AcceptingUser { get; set; }
        public IList<InviteReview> Reviews { get; set; } = new List<InviteReview>();
    }

    public class InviteReview
    {
        public string Type { get; set; }
        public int DateReview { get; set; }
        public ConnectedTeam ReviewingTeam { get; set; }
    }
}