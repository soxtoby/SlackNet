namespace SlackNet
{
    [SlackType("file")]
    public class ReactionFile : ReactionItem
    {
        public string File { get; set; }
    }
}