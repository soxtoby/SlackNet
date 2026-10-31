using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AuthTeamsListResponse : SlackObject
{
    public IList<AuthTeam> Teams { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

public class AuthTeam : SlackObject
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
} 