using System;

namespace SlackNet.Blocks
{
    [SlackType("datepicker")]
    public class DatePickerAction : BlockAction
    {
        public DateTime SelectedDate { get; set; }
    }
}