namespace SlackNet.Events
{
    /// <summary>
    /// Verifies ownership of an Events API Request URL.
    /// </summary>
    public class UrlVerification : EventRequest
    {
        public string Challenge { get; set; }
    }
}