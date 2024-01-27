using System.Collections.Generic;

namespace SlackNet.Events;

public class TeamAccessRevoked : Event
{
    public IList<string> TeamIds { get; set; } = [];
}