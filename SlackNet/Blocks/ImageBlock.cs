namespace SlackNet.Blocks;

/// <summary>
/// Displays an image.<br />
/// A simple image block, designed to make those cat photos really pop.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/#image">Slack documentation</a> for more information.</remarks>
[SlackType("image")]
public class ImageBlock : Block
{
    public ImageBlock() : base("image") { }

    /// <summary>
    /// The URL of the image to be displayed.
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// A plain-text summary of the image. This should not contain any markup.
    /// </summary>
    public string AltText { get; set; }

    /// <summary>
    /// A Slack image file object that defines the source of the image.
    /// </summary>
    public ImageFileReference SlackFile { get; set; }

    /// <summary>
    /// An optional title for the image.
    /// </summary>
    public PlainText Title { get; set; }
}