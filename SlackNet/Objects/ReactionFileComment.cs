namespace SlackNet.Objects
{
    [SlackType("file_comment")]
    public class ReactionFileComment : ReactionItem
    {
        public string FileComment { get; set; }
        public string File { get; set; }
    }
}