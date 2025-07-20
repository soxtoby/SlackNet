using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ExternalTeamsListResponse
{
    public IList<ExternalTeam> Organizations { get; set; }
    public int TotalCount { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; }
}