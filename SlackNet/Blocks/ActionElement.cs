namespace SlackNet.Blocks
{
    public interface IActionElement
    {
        string Type { get; }
        string ActionId { get; set; }
    }

    public abstract class ActionElement : BlockElement, IActionElement
    {
        protected ActionElement(string type) : base(type) { }

        /// <summary>
        /// An identifier for this action. You can use this when you receive an interaction payload to identify the source of the action.
        /// Should be unique among all other <see cref="ActionId"/>s used elsewhere by your app. 
        /// </summary>
        public string ActionId { get; set; }

        /// <summary>
        /// Defines an optional confirmation dialog that appears after the element is activated.
        /// </summary>
        public ConfirmationDialog Confirm { get; set; }
    }
}