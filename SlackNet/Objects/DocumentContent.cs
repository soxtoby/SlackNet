#nullable enable
namespace SlackNet;

public class DocumentContent : SlackObject
{
    public string Type { get; set; } = "markdown";
    public string Markdown { get; set; } = string.Empty;
}