namespace SlackNet.Interaction
{
    [SlackType("interactive_message")]
    public class OptionsRequest : OptionsRequestBase
    {
        public string Name { get; set; }
        public string CallbackId { get; set; }
        public string ActionTs { get; set; }
        public string MessageTs { get; set; }
        public string AttachmentId { get; set; }
    }
}