#nullable enable
namespace SlackNet.WebApi;

public class ApprovalMessage
{
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// If set to True, <see cref="Text"/> will override the original invitation message.
    /// Otherwise, <see cref="Text"/> will be appended to the original invitation message.
    /// </summary>
    public bool IsOverride { get; set; }
}