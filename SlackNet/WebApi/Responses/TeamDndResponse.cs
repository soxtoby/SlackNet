using System.Collections.Generic;

namespace SlackNet.WebApi
{
    class TeamDndResponse
    {
        public Dictionary<string, DndStatus> Users { get; } = new();
    }
}