using System;

namespace SlackNet
{
    [Obsolete("Use Conversation instead")]
    public class Im : Hub
    {
        public Im() => IsMember = true;

        public string User { get; set; }
        public bool IsUserDeleted { get; set; }

        public override string ToString() => Link.User(User).ToString();
    }
}