using System;
using Newtonsoft.Json;

namespace SlackNet;

public class ConnectedTeam
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public Icons? Icon { get; set; }
    public bool IsVerified { get; set; }
    public string? Domain { get; set; }
    public int DateCreated { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();
    public string? AvatarBaseUrl { get; set; }
    public bool RequiresSponsorship { get; set; }
}