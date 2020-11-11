namespace SlackNet
{
    public class WorkflowOutput
    {
        public WorkflowOutputType Type { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }
    }

    public enum WorkflowOutputType
    {
        Text,
        Channel,
        User
    }
}