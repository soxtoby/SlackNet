#nullable enable
using System;
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class SlackbotToolUsageResponse
{
    /// <summary>
    /// The date the rows cover, in UTC: the <c>date</c> argument, the date the <c>cursor</c> was issued for,
    /// or the latest date with available data when both were omitted.
    /// Null when no Slackbot analytics data is available yet.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// The dates this method has data for. <see cref="DateRange.StartDate"/> is the earliest date it accepts
    /// and <see cref="DateRange.EndDate"/> the latest, which is also the default.
    /// Null when no Slackbot analytics data is available yet.
    /// </summary>
    public DateRange? AvailableDateRange { get; set; }

    /// <summary>
    /// One row per user for whom Slackbot used at least one user-facing tool on the date.
    /// When a page ends partway through a user's tools, the rest continue in a row with the same <see cref="SlackbotToolUsage.UserId"/>
    /// at the start of the next page, so merge rows that share it.
    /// </summary>
    public IList<SlackbotToolUsage> ToolUsage { get; set; } = [];

    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
