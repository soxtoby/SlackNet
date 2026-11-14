#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class OriginalConnectedChannelInfoListResponse
{
    public IList<OriginalConnectedChannelInfo> Channels { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
