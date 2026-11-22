using System.Collections.Generic;

namespace SlackNet.WebApi;

public class RestrictedAppListResponse
{
    public IList<AppResolution> RestrictedApps { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
