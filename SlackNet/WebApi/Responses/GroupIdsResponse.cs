using System.Collections.Generic;

namespace SlackNet.WebApi;

record GroupIdsResponse
{
    public IList<string> GroupIds { get; init; } = [];
}
