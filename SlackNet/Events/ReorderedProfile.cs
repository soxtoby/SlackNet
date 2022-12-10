using System.Collections.Generic;

namespace SlackNet.Events;

public class ReorderedProfile
{
    public IList<ReorderedProfileField> Fields { get; set; }
}