using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AppRequestListResponse
{
    public IList<AppRequest> AppRequests { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
