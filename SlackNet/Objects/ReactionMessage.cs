namespace SlackNet.Objects
{
    [SlackType("message")]
    public class ReactionMessage : ReactionItem
    {
        public string Channel { get; set; }
        public string Ts { get; set; }
    }
}