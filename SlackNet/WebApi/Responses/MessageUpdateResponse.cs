namespace SlackNet.WebApi.Responses
{
    public class MessageUpdateResponse
    {
        public string Channel { get; set; }
        public string Ts { get; set; }
        public string Text { get; set; }
    }
}