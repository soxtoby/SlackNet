using System.Collections.Generic;

namespace SlackNet.Interaction;

public class BlockOptionsResponse : SlackObject
{
    public IList<Blocks.Option> Options { get; set; }
    public IList<Blocks.OptionGroup> OptionGroups { get; set; }
}