using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IRemindersApi
    {
        /// <summary>
        /// Creates a reminder.
        /// </summary>
        /// <param name="text">The content of the reminder.</param>
        /// <param name="time">When this reminder should happen (up to five years from now).</param>
        /// <param name="userId">The user who will receive the reminder. If no user is specified, the reminder will go to user who created it.</param>
        /// <param name="cancellationToken"></param>
        Task<Reminder> Add(string text, DateTime time, string userId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a reminder.
        /// </summary>
        /// <param name="text">The content of the reminder.</param>
        /// <param name="time">When this reminder should happen (up to 24 hours from now).</param>
        /// <param name="userId">The user who will receive the reminder. If no user is specified, the reminder will go to user who created it.</param>
        /// <param name="cancellationToken"></param>
        Task<Reminder> Add(string text, TimeSpan time, string userId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a reminder.
        /// </summary>
        /// <param name="text">The content of the reminder.</param>
        /// <param name="time">
        /// When this reminder should happen: the Unix timestamp (up to five years from now), 
        /// the number of seconds until the reminder (if within 24 hours), 
        /// or a natural language description (Ex. "in 15 minutes," or "every Thursday")
        /// </param>
        /// <param name="userId">The user who will receive the reminder. If no user is specified, the reminder will go to user who created it.</param>
        /// <param name="cancellationToken"></param>
        Task<Reminder> Add(string text, string time, string userId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Completes a reminder.
        /// </summary>
        /// <param name="reminderId">The ID of the reminder to be marked as complete.</param>
        /// <param name="cancellationToken"></param>
        Task Complete(string reminderId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Deletes a reminder.
        /// </summary>
        /// <param name="reminderId">The ID of the reminder.</param>
        /// <param name="cancellationToken"></param>
        Task Delete(string reminderId, CancellationToken? cancellationToken);

        /// <summary>
        /// Returns information about a reminder.
        /// </summary>
        /// <param name="reminderId">The ID of the reminder.</param>
        /// <param name="cancellationToken"></param>
        Task<Reminder> Info(string reminderId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Lists all reminders created by or for the user.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<Reminder>> List(CancellationToken? cancellationToken = null);
    }

    public class RemindersApi : IRemindersApi
    {
        private readonly ISlackApiClient _client;
        public RemindersApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Creates a reminder.
        /// </summary>
        /// <param name="text">The content of the reminder.</param>
        /// <param name="time">When this reminder should happen (up to five years from now).</param>
        /// <param name="userId">The user who will receive the reminder. If no user is specified, the reminder will go to user who created it.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Reminder> Add(string text, DateTime time, string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ReminderResponse>("reminders.add", new Args
                {
                    { "text", text },
                    { "time", time.ToTimestamp() },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Reminder;

        /// <summary>
        /// Creates a reminder.
        /// </summary>
        /// <param name="text">The content of the reminder.</param>
        /// <param name="time">When this reminder should happen (up to 24 hours from now).</param>
        /// <param name="userId">The user who will receive the reminder. If no user is specified, the reminder will go to user who created it.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Reminder> Add(string text, TimeSpan time, string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ReminderResponse>("reminders.add", new Args
                {
                    { "text", text },
                    { "time", time.ToOffset(TimeSpan.FromHours(24)) },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Reminder;

        /// <summary>
        /// Creates a reminder.
        /// </summary>
        /// <param name="text">The content of the reminder.</param>
        /// <param name="time">
        /// When this reminder should happen: the Unix timestamp (up to five years from now), 
        /// the number of seconds until the reminder (if within 24 hours), 
        /// or a natural language description (Ex. "in 15 minutes," or "every Thursday")
        /// </param>
        /// <param name="userId">The user who will receive the reminder. If no user is specified, the reminder will go to user who created it.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Reminder> Add(string text, string time, string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ReminderResponse>("reminders.add", new Args
                {
                    { "text", text },
                    { "time", time },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Reminder;

        /// <summary>
        /// Completes a reminder.
        /// </summary>
        /// <param name="reminderId">The ID of the reminder to be marked as complete.</param>
        /// <param name="cancellationToken"></param>
        public Task Complete(string reminderId, CancellationToken? cancellationToken = null) =>
            _client.Get("reminders.complete", new Args { { "reminder", reminderId } }, cancellationToken);

        /// <summary>
        /// Deletes a reminder.
        /// </summary>
        /// <param name="reminderId">The ID of the reminder.</param>
        /// <param name="cancellationToken"></param>
        public Task Delete(string reminderId, CancellationToken? cancellationToken = null) =>
            _client.Get("reminders.delete", new Args { { "reminder", reminderId } }, cancellationToken);

        /// <summary>
        /// Returns information about a reminder.
        /// </summary>
        /// <param name="reminderId">The ID of the reminder.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Reminder> Info(string reminderId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<ReminderResponse>("reminders.info", new Args { { "reminder", reminderId } }, cancellationToken).ConfigureAwait(false)).Reminder;

        /// <summary>
        /// Lists all reminders created by or for the user.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<Reminder>> List(CancellationToken? cancellationToken = null) =>
            (await _client.Get<ReminderListResponse>("reminders.list", new Args(), cancellationToken).ConfigureAwait(false)).Reminders;
    }
}