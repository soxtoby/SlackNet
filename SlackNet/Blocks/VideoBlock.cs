namespace SlackNet.Blocks
{
    /// <summary>
    /// A video block is designed to embed videos.
    /// </summary>
    [SlackType("video")]
    public class VideoBlock : Block
    {
        public VideoBlock() : base("video") { }

        /// <summary>
        /// A tooltip for the video. Required for accessibility.
        /// </summary>
        public string AltText { get; set; } = string.Empty;
        
        /// <summary>
        /// Author name to be displayed. Must be less than 50 characters.
        /// </summary>
        public string AuthorName { get; set; }
        
        /// <summary>
        /// Description for video.
        /// </summary>
        public PlainText Description { get; set; }
        
        /// <summary>
        /// Icon for the video provider - e.g. Youtube icon.
        /// </summary>
        public string ProviderIconUrl { get; set; }

        /// <summary>
        /// The originating application or domain of the video e.g. Youtube.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Video title in plain text format. Must be less than 200 characters. Required.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Hyperlink for the title text. Must correspond to the non-embeddable URL for the video. Must go to an HTTPS URL.
        /// </summary>
        public string TitleUrl { get; set; }

        /// <summary>
        /// The thumbnail image URL. Required.
        /// </summary>
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// The URL to be embedded. Must match any existing unfurl domains within the app and point to a HTTPS URL. Required.
        /// </summary>
        public string VideoUrl { get; set; } = string.Empty;
    }
}