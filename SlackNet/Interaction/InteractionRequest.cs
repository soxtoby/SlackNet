namespace SlackNet.Interaction
{
    public abstract class InteractionRequest
    {
        public string Type { get; set; }
        public string Token { get; set; }
        public string CallbackId { get; set; }
        public Team Team { get; set; }
        public Channel Channel { get; set; }
        public User User { get; set; }
        public string ActionTs { get; set; }
    }
}