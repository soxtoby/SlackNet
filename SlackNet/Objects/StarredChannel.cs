namespace SlackNet
{
    [SlackType("channel")]
    public class StarredChannel : StarredItem
    {
        public string Channel { get; set; }
    }
}