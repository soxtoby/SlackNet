using System.Collections.Generic;

namespace SlackNet.Blocks
{
    [SlackType("multi_static_select")]
    public class StaticMultiSelectAction : BlockAction
    {
        public IList<Option> SelectedOptions { get; set; } = new List<Option>();
    }
}