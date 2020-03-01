namespace SlackNet.Interaction
{
    public class PushViewResponse : ViewSubmissionResponse
    {
        public PushViewResponse() : base("push") { }

        public ViewDefinition View { get; set; }
    }
}