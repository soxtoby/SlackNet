using System.Collections.Generic;

namespace SlackNet.Blocks
{
    [SlackType("multi_channels_select")]
    public class ChannelMultiSelectAction
    {
        public IList<string> SelectedChannels { get; set; } = new List<string>();
    }
}