using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AccessLogsResponse
{
    public IList<Login> Logins { get; set; } = [];
    public Paging Paging { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}