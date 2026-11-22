using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ActiveSessionListResponse
{
    public IList<ActiveSession> ActiveSessions { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
