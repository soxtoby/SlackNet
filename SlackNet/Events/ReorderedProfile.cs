using System.Collections.Generic;

namespace SlackNet.Events;

public class ReorderedProfile : SlackObject
{
    public IList<ReorderedProfileField> Fields { get; set; }
}