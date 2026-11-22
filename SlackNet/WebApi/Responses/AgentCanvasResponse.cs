using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AgentCanvasResponse
{
    /// <summary>
    /// The encoded ID of the canvas.
    /// </summary>
    public required string CanvasId { get; set; }

    /// <summary>
    /// The title of the canvas.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The full canvas content as markdown.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Comment threads on the canvas, ordered oldest first.
    /// </summary>
    public IList<CanvasComment> Comments { get; set; } = [];

    /// <summary>
    /// True if the canvas had more comment threads than could be returned, and older threads were dropped from <see cref="Comments"/>.
    /// </summary>
    public bool HasMoreComments { get; set; }
}
