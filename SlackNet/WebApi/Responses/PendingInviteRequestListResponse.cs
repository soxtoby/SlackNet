#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class PendingInviteRequestListResponse
{
    public IList<InviteRequest> InviteRequests { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
