#nullable disable
using System;
using Newtonsoft.Json;

namespace SlackNet;

public class Invite
{
    public string Id { get; set; }
    public int DateCreated { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();
    public int DateInvalid { get; set; }
    [JsonIgnore]
    public DateTime? Expires => DateInvalid.ToDateTime();
    public ConnectedTeam InvitingTeam { get; set; }
    public User InvitingUser { get; set; }
    public string RecipientUserId { get; set; }
    public string Link { get; set; }
}