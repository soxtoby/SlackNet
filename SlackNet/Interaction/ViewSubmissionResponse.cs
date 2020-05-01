namespace SlackNet.Interaction
{
    public abstract class ViewSubmissionResponse
    {
        protected ViewSubmissionResponse(string responseAction) => ResponseAction = responseAction;

        public string ResponseAction { get; set; }
        
        public static ViewSubmissionResponse Null => new NullResponse();

        class NullResponse : ViewSubmissionResponse
        {
            public NullResponse() : base(null) { }
        }
    }
}