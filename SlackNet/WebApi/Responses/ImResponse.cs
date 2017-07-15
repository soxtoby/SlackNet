namespace SlackNet.WebApi
{
    public class ImResponse
    {
        public bool NoOp { get; set; }
        public bool AlreadyOpen { get; set; }
        public Im Channel { get; set; }
    }
}