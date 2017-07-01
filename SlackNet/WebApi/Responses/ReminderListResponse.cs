using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class ReminderListResponse
    {
        public List<Reminder> Reminders { get; } = new List<Reminder>();
    }
}