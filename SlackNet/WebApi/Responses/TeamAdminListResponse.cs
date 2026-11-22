using System.Collections.Generic;

namespace SlackNet.WebApi;

public class TeamAdminListResponse
{
    public IList<string> AdminIds { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
