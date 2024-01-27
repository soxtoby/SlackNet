using System.Collections.Generic;

namespace SlackNet.Events;

public class TeamAccessGranted : Event
{
    public IList<string> TeamIds { get; set; } = [];
}