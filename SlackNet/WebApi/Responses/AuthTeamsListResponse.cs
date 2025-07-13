using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AuthTeamsListResponse
{
    public IList<AuthTeam> Teams { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

public class AuthTeam
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
} 