#nullable disable
using System.Collections.Generic;

namespace SlackNet.WebApi;

record AgentViewListResponse
{
    public IList<AgentView> Views { get; init; } = [];
}
