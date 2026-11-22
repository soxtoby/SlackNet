using System.Collections.Generic;

namespace SlackNet.WebApi;

public class EnterpriseTeamListResponse
{
    public IList<EnterpriseTeam> Teams { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
