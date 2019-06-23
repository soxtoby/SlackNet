namespace SlackNet.Blocks
{
    [SlackType("static_select")]
    public class StaticSelectAction : BlockAction
    {
        public Option SelectedOption { get; set; }
    }
}