#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class InviteRequestListResponse
{
    public IList<Invite> InviteRequests { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}