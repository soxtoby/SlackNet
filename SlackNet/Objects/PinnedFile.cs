namespace SlackNet.Objects
{
    [SlackType("file")]
    public class PinnedFile : PinnedItem
    {
        public File File { get; set; }
    }
}