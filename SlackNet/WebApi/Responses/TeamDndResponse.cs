using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class TeamDndResponse
    {
        public Dictionary<string, DndStatus> Users { get; } = new Dictionary<string, DndStatus>();
    }
}