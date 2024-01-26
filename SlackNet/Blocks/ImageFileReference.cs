namespace SlackNet.Blocks;

/// <summary>
/// Defines an object containing Slack file information to be used in an image block or image element.
/// </summary>
public class ImageFileReference
{
    /// <summary>
    /// This URL can be the <see cref="File.UrlPrivate"/> or the <see cref="File.Permalink"/> of the Slack <see cref="File"/>.
    /// </summary>
    public string Url { get; set; }
    
    /// <summary>
    /// Slack <see cref="File.Id"/> of the <see cref="File"/>.
    /// </summary>
    public string Id { get; set; }
}