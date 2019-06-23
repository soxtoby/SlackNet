namespace SlackNet.Blocks
{
    [SlackType("button")]
    public class ButtonAction : BlockAction
    {
        public TextObject Text { get; set; }
        public string Value { get; set; }
    }
}