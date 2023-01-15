using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackNet.Blocks;

/// <summary>
/// An element which lets users easily select a date from a calendar style UI.
/// </summary>
[SlackType("datetimepicker")]
public class DateTimePicker : ActionElement, IInputBlockElement
{
    public DateTimePicker() : base("datetimepicker") { }

    /// <summary>
    /// The initial date and time that is selected when the element is loaded.
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? InitialDateTime { get; set; }

    /// <summary>
    /// Indicates whether the element will be set to auto focus within the <see cref="ViewInfo"/> object. Only one element can be set to true.
    /// </summary>
    public bool FocusOnLoad { get; set; }
}

[SlackType("datetimepicker")]
public class DateTimePickerAction : BlockAction
{
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? SelectedDateTime { get; set; }
}

[SlackType("datetimepicker")]
public class DateTimePickerValue : ElementValue
{
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? SelectedDateTime { get; set; }
}