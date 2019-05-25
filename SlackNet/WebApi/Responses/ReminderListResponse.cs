using System.Collections.Generic;

namespace SlackNet.WebApi
{
    class ReminderListResponse
    {
        public List<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}