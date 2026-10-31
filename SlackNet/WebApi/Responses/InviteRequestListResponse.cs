#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class InviteRequestListResponse : SlackObject
{
    public IList<Invite> InviteRequests { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}