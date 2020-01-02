using System.Collections.Generic;

namespace SlackNet.Blocks
{
    [SlackType("multi_external_select")]
    public class ExternalMultiSelectAction
    {
        public IList<Option> SelectedOptions { get; set; } = new List<Option>();
    }
}