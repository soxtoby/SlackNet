using System;

namespace SlackNet.Blocks
{
    /// <summary>
    /// An element which lets users easily select a date from a calendar style UI.
    /// Date picker elements can be used inside of section and actions blocks.
    /// </summary>
    [SlackType("datepicker")]
    public class DatePicker : ActionElement, IInputBlockElement
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
    }

    [SlackType("datepicker")]
    public class DatePickerAction : BlockAction
    {
        public DateTime? SelectedDate { get; set; }
    }

    [SlackType("datepicker")]
    public class DatePickerValue : ElementValue
    {
        public DateTime? SelectedDate { get; set; }
    }
}