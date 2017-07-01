namespace SlackNet.Objects
{
    [SlackType("channel")]
    public class StarredChannel : StarredItem
    {
        public string Channel { get; set; }
    }
}