#nullable disable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AppActivityListResponse
{
    public IList<AppActivity> Activities { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
