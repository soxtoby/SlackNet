namespace SlackNet.Objects
{
    [SlackType("group")]
    public class StarredGroup : StarredItem
    {
        public string Group { get; set; }
    }
}