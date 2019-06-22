using System;

namespace SlackNet.Blocks
{
    /// <summary>
    /// An element which lets users easily select a date from a calendar style UI.
    /// Date picker elements can be used inside of section and actions blocks.
    /// </summary>
    [SlackType("datepicker")]
    public class DatePicker : BlockElement, IActionElement
    {
        public DatePicker() : base("datepicker") { }

        /// <summary>
        /// A plain text object that defines the placeholder text shown on the menu. 
        /// </summary>
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// The initial date that is selected when the element is loaded.
        /// </summary>
        public DateTime? InitialDate { get; set; }

        /// <summary>
        /// Defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        public ConfirmationDialog Confirm { get; set; }

        /// <summary>
        /// An identifier for this action. You can use this when you receive an interaction payload to identify the source of the action.
        /// Should be unique among all other <see cref="ActionId"/>s used elsewhere by your app. 
        /// </summary>
        public string ActionId { get; set; }
    }
}