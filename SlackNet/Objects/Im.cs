namespace SlackNet
{
    public class Im : Conversation
    {
        public Im() => IsMember = true;

        public override string ToString() => Link.User(User).ToString();
    }
}