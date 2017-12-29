namespace SlackNet.Events
{
    /// <summary>
    /// Verifies ownership of an Events API Request URL.
    /// </summary>
    public class UrlVerification : Event
    {
        public string Token { get; set; }
        public string Challenge { get; set; }
    }
}