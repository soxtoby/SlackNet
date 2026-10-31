using System.Collections.Generic;

namespace SlackNet.Interaction;

public class OptionsResponse : SlackObject
{
    public IList<Option> Options { get; set; }
    public IList<OptionGroup> OptionGroups { get; set; }
}