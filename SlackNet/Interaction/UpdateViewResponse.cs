namespace SlackNet.Interaction
{
    public class UpdateViewResponse : ViewSubmissionResponse
    {
        public UpdateViewResponse() : base("update") { }

        public ViewDefinition View { get; set; }
    }
}