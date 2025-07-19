#nullable enable
namespace SlackNet;

public class CanvasChange
{
    public CanvasOperation Operation { get; set; }
    public DocumentContent? DocumentContent { get; set; }
    public string? SectionId { get; set; }
}

public enum CanvasOperation
{
    InsertAfter,
    InsertBefore,
    InsertAtStart,
    InsertAtEnd,
    Replace,
    Delete
}