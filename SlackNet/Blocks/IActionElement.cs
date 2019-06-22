namespace SlackNet.Blocks
{
    public interface IActionElement
    {
        string Type { get; }
        string ActionId { get; set; }
    }
}