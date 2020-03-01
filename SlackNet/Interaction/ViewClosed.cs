namespace SlackNet.Interaction
{
    public class ViewClosed : InteractionRequest
    {
        /// <summary>
        /// The source modal view that the user dismissed.
        /// This will include the full state of the view, including any input blocks and their current values.
        /// </summary>
        public ViewInfo View { get; set; }
        
        /// <summary>
        /// A boolean that represents whether or not a whole view stack was cleared.
        /// </summary>
        public bool IsCleared { get; set; }
    }
}