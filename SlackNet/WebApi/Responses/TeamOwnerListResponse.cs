#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class TeamOwnerListResponse
{
    public IList<string> OwnerIds { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
