namespace SlackNet.Objects
{
    [SlackType("file_comment")]
    public class PinnedFileComment : PinnedItem
    {
        public File File { get; set; }
        public FileComment Comment { get; set; }
    }
}