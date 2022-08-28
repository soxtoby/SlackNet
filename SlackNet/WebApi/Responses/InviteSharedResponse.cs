namespace SlackNet.WebApi
{
    public class InviteSharedResponse
    {
        public string InviteId { get; set; }
        public bool IsLegacySharedChannel { get; set; }
        public string ConfCode { get; set; }
        public string Url { get; set; }
    }
}