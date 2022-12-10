namespace SlackNet.Blocks;

/// <summary>
/// Displays a remote file.
/// </summary>
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