namespace SlackNet.Blocks
{
    [SlackType("channels_select")]
    public class ChannelSelectAction : BlockAction
    {
        public string SelectedChannel { get; set; }
    }
}