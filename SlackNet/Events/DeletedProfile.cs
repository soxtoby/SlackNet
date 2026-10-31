using System.Collections.Generic;

namespace SlackNet.Events;

public class DeletedProfile : SlackObject
{
    public IList<string> Fields { get; set; }
}