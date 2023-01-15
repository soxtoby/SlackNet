using System;

namespace SlackNet.Blocks;

/// <summary>
/// An element which lets users easily select a date from a calendar style UI.
/// </summary>
[SlackType("datepicker")]
public class DatePicker : ActionElement, IInputBlockElement
{
    public DatePicker() : base("datepicker") { }

    /// <summary>
    /// A plain text object that defines the placeholder text shown on the datepicker.
    /// </summary>
    public PlainText Placeholder { get; set; }

    /// <summary>
    /// The initial date that is selected when the element is loaded.
    /// </summary>
    public DateTime? InitialDate { get; set; }

    /// <summary>
    /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
    /// </summary>
    [IgnoreIfDefault] // See issue #138 - Slack returns an internal error when this is included in a HomeViewDefinition
    public bool FocusOnLoad { get; set; }
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