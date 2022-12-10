using System.Collections.Generic;

namespace SlackNet.Interaction;

public class OptionsResponse
{
    public IList<Option> Options { get; set; }
    public IList<OptionGroup> OptionGroups { get; set; }
}