using System;

namespace SlackNet.Blocks
{
    /// <summary>
    /// An element which allows selection of a time of day.
    /// </summary>
    [SlackType("timepicker")]
    public class TimePicker : ActionElement, IInputBlockElement
    {
        public TimePicker() : base("timepicker") { }

        /// <summary>
        /// A plain text object that defines the placeholder text shown on the timepicker.
        /// </summary>
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// The initial time that is selected when the element is loaded.
        /// </summary>
        public TimeSpan? InitialTime { get; set; }

        /// <summary>
        /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
        /// </summary>
        public bool FocusOnLoad { get; set; }
    }

    [SlackType("timepicker")]
    public class TimePickerAction : BlockAction
    {
        public TimeSpan? SelectedTime { get; set; }
    }

    [SlackType("timepicker")]
    public class TimePickerValue : ElementValue
    {
        public TimeSpan? SelectedTime { get; set; }
    }
}