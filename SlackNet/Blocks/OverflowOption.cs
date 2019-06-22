namespace SlackNet.Blocks
{
    /// <summary>
    /// An object that represents a single selectable item in an overflow menu.
    /// </summary>
    public class OverflowOption : Option
    {
        /// <summary>
        /// A URL to load in the user's browser when the option is clicked.
        /// If you're using url, you'll still receive an interaction payload and will need to send an acknowledgement response.
        /// </summary>
        public string Url { get; set; }
    }
}