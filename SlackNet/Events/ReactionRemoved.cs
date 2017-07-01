using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connected clients for users who can see the content that had the reaction when a reaction is removed from an item.
    /// </summary>
    public class ReactionRemoved : Event
    {
        /// <summary>
        /// Indicates the ID of the user who performed this event.
        /// </summary>
        public string User { get; set; }
        public string Reaction { get; set; }
        /// <summary>
        /// The ID of the user that created the original item that has been reacted to.
        /// </summary>
        public string ItemUser { get; set; }
        public ReactionItem Item { get; set; }
        public string EventTs { get; set; }
    }
}