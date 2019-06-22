namespace SlackNet.Blocks
{
    /// <summary>
    /// An element to insert an image - this element can be used in <see cref="SectionBlock"/> and <see cref="ContextBlock"/> blocks only.
    /// If you want a block with only an image in it, you're looking for the <see cref="ImageBlock"/> block.
    /// </summary>
    public class Image : BlockElement, IContextElement
    {
        public Image() : base("image") { }

        /// <summary>
        /// The URL of the image to be displayed.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// A plain-text summary of the image. This should not contain any markup.
        /// </summary>
        public string AltText { get; set; }
    }
}