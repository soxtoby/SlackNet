namespace SlackNet
{
    [SlackType("file_comment")]
    public class StarredFileComment : StarredItem
    {
        public File File { get; set; }
        public FileComment Comment { get; set; }
    }
}