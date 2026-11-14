#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class DeniedInviteRequestListResponse
{
    public IList<DeniedInviteRequest> DeniedRequests { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
