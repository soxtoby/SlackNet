namespace SlackNet
{
    public class WorkSpace
    {
        public string Id { get; set; }
#nullable enable
            // A single example from Slack with no description exists where name is missing from their response.
        public string? Name { get; set; }
#nullable disable
    }
}
