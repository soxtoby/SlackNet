#nullable enable
namespace SlackNet.WebApi;

public class AgentViewResponse
{
    /// <summary>
    /// The encoded ID of the code channel the view belongs to.
    /// </summary>
    public required string ChannelId { get; set; }

    /// <summary>
    /// The encoded channel tab ID of the view.
    /// </summary>
    public required string ViewId { get; set; }

    /// <summary>
    /// The encoded ID of the backing file for the view.
    /// For HTML and Block Kit views this is the agent-owned content file; for diff views it is the diff snippet file;
    /// for canvas views it is the canvas's file ID (same value as <see cref="CanvasId"/>).
    /// </summary>
    public required string FileId { get; set; }

    /// <summary>
    /// Server-assigned content version, monotonically increasing per view.
    /// </summary>
    public int ContentVersion { get; set; }

    /// <summary>
    /// The type of the view that was created or updated.
    /// </summary>
    public AgentViewType Type { get; set; }

    /// <summary>
    /// For canvas views, the encoded ID of the attached canvas. Absent for other view types.
    /// </summary>
    public string? CanvasId { get; set; }
}
