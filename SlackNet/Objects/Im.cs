namespace SlackNet
{
    public class Im : Conversation
    {
        public Im() => IsMember = true;

        public string User { get; set; }
        public bool IsUserDeleted { get; set; }

        public override string ToString() => Link.User(User).ToString();
    }
}