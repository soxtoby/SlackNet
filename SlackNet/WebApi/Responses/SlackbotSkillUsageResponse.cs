#nullable enable
using System;
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class SlackbotSkillUsageResponse
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
    /// One row per user who used, added, or shared at least one skill on the date.
    /// When a page ends partway through a user's skills, the rest continue in a row with the same <see cref="SlackbotSkillUsage.UserId"/>
    /// at the start of the next page, so merge rows that share it.
    /// </summary>
    public IList<SlackbotSkillUsage> SkillUsage { get; set; } = [];

    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
