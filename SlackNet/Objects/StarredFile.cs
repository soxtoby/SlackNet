namespace SlackNet
{
    [SlackType("file")]
    public class StarredFile : StarredItem
    {
        public File File { get; set; }
    }
}