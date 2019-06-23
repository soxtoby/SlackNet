namespace SlackNet.Interaction
{
    public class OptionsRequestBase
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public Team Team { get; set; }
        public Channel Channel { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
    }
}