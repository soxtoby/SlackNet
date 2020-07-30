namespace SlackNet.Blocks
{
    /// <summary>
    /// A header is a plain-text block that displays in a larger, bold font. 
    /// Use it to delineate between different groups of content in your app's surfaces.
    /// </summary>
    [SlackType("header")]
    public class HeaderBlock : Block
    {
        public HeaderBlock() : base("header") { }

        /// <summary>
        /// The text for the block.
        /// </summary>
        public PlainText Text { get; set; }
    }
}