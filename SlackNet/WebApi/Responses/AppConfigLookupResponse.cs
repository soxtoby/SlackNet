#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

record AppConfigLookupResponse
{
    public IList<AppConfig> Configs { get; init; } = [];
}
