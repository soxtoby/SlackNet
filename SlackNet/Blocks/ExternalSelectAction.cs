namespace SlackNet.Blocks
{
    [SlackType("external_select")]
    public class ExternalSelectAction : BlockAction
    {
        public Option SelectedOption { get; set; }
        public PlainText Placeholder { get; set; }
    }
}