namespace SlackNet.Blocks;

/// <summary>
/// A simple image block, designed to make those cat photos really pop.
/// </summary>
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
    /// An optional title for the image.
    /// </summary>
    public PlainText Title { get; set; }
}