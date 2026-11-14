#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ApprovedInviteRequestListResponse
{
    public IList<ApprovedInviteRequest> ApprovedRequests { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
