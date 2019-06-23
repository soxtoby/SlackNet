namespace SlackNet.Blocks
{
    [SlackType("overflow")]
    public class OverflowAction : BlockAction
    {
        public Option SelectedOption { get; set; }
    }
}