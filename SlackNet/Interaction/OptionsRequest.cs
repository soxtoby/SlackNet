namespace SlackNet.Interaction
{
    public class OptionsRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string CallbackId { get; set; }
        public Team Team { get; set; }
        public Channel Channel { get; set; }
        public User User { get; set; }
        public string ActionTs { get; set; }
        public string MessageTs { get; set; }
        public string AttachmentId { get; set; }
        public string Token { get; set; }
    }
}