namespace SlackNet.Interaction
{
    [SlackType("shortcut")]
    public class GlobalShortcut : InteractionRequest
    {
        /// <summary>
        /// An ID that you defined when creating the shortcut.
        /// </summary>
        public string CallbackId { get; set; }

        /// <summary>
        /// A temporary ID generated for the interaction in your app. This value can be used to open modals.
        /// </summary>
        public string TriggerId { get; set; }
    }
}