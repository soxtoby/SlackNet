using System.Collections.Generic;

namespace SlackNet.Events;

public class DeletedProfile
{
    public IList<string> Fields { get; set; }
}