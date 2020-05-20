namespace SlackNet
{
    public class Channel : Conversation
    {
        public override string ToString() => Link.Channel(Id).ToString();
    }
}