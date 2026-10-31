#nullable enable
namespace SlackNet.WebApi;

public class ExternalFileReference : SlackObject
{
    public required string Id { get; set; }
    public string? Title { get; set; }
}