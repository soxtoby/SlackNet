using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class ConnectInvitesListResponse
{
    public IList<PendingInvite> Invites { get; set; } = null!;
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

public class PendingInvite
{
    public string? Direction { get; set; }
    public string? Status { get; set; }
    public int DateLastUpdated { get; set; }
    [JsonIgnore]
    public DateTime LastUpdated => DateLastUpdated.ToDateTime().GetValueOrDefault();
    public string? InviteType { get; set; }
    public Invite? Invite { get; set; }
    public InviteChannel? Channel { get; set; }
    public IList<InviteAcceptance> Acceptances { get; set; } = [];
}

public class InviteAcceptance
{
    public string? ApprovalStatus { get; set; }
    public int DateAccepted { get; set; }
    [JsonIgnore]
    public DateTime? Accepted => DateAccepted.ToDateTime();
    public int DateInvalid { get; set; }
    [JsonIgnore]
    public DateTime? Expires => DateInvalid.ToDateTime();
    public int DateLastUpdated { get; set; }
    [JsonIgnore]
    public DateTime LastUpdated => DateLastUpdated.ToDateTime().GetValueOrDefault();
    public ConnectedTeam? AcceptingTeam { get; set; }
    public User? AcceptingUser { get; set; }
    public IList<InviteReview> Reviews { get; set; } = [];
}

public class InviteReview
{
    public string? Type { get; set; }
    public int DateReview { get; set; }
    [JsonIgnore]
    public DateTime Reviewed => DateReview.ToDateTime().GetValueOrDefault();
    public ConnectedTeam? ReviewingTeam { get; set; }
}