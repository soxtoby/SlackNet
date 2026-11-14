#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AppMcpServerListResponse
{
    public IList<AppMcpServer> Servers { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
