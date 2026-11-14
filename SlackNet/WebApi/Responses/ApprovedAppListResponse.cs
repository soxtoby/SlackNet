#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ApprovedAppListResponse
{
    public IList<AppResolution> ApprovedApps { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
