namespace SlackNet.Objects
{
    [SlackType("im")]
    public class StarredIm : StarredItem
    {
        public string Channel { get; set; }
    }
}