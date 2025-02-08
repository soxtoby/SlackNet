namespace SlackNet.Blocks;

/// <summary>
/// Displays a remote file.<br />
/// You can't add this block to app surfaces directly, but it will show up when retrieving messages that contain remote files.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#file">Slack documentation</a> for more information.</remarks>
[SlackType("file")]
public class FileBlock : Block
{
    public FileBlock() : base("file") { }

    /// <summary>
    /// The external unique ID for this file.
    /// </summary>
    public string ExternalId { get; set; }

    /// <summary>
    /// At the moment, <see cref="Source"/> will always be <c>remote</c> for a remote file.
    /// </summary>
    public string Source { get; set; } = "remote";
}