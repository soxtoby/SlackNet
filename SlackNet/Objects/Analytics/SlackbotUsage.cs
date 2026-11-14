#nullable enable
using System;
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Skills a user used, added, or shared with Slackbot on a single date.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.slackbot.skills/">Slack documentation</a> for more information.</remarks>
public class SlackbotSkillUsage
{
    /// <summary>
    /// The date this row of data is representative of, in UTC.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Unique ID of the involved Enterprise organization.
    /// </summary>
    public required string EnterpriseId { get; set; }

    /// <summary>
    /// The canonical, organization-wide user ID this row concerns.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// The email address of record for the same user.
    /// Omitted when the organization hides email addresses, and for guests when the organization hides guest email addresses.
    /// </summary>
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Skills the user used, added, or shared with Slackbot on this date.
    /// </summary>
    public IList<SkillUsage> Skills { get; set; } = [];
}

/// <summary>
/// How many times a user used, added, and shared a Slackbot skill on a single date.
/// </summary>
public class SkillUsage
{
    /// <summary>
    /// The file ID of the skill, stable across renames.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The name of the skill. Empty when the skill has been deleted.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Number of times the user invoked the skill with Slackbot on this date.
    /// </summary>
    public int UseCount { get; set; }

    /// <summary>
    /// Number of times the user added the skill on this date.
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// Number of times the user shared the skill on this date.
    /// </summary>
    public int SharedCount { get; set; }
}

/// <summary>
/// User-facing tools Slackbot used on a user's behalf on a single date.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.slackbot.tools/">Slack documentation</a> for more information.</remarks>
public class SlackbotToolUsage
{
    /// <summary>
    /// The date this row of data is representative of, in UTC.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Unique ID of the involved Enterprise organization.
    /// </summary>
    public required string EnterpriseId { get; set; }

    /// <summary>
    /// The canonical, organization-wide user ID this row concerns.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// The email address of record for the same user.
    /// Omitted when the organization hides email addresses, and for guests when the organization hides guest email addresses.
    /// </summary>
    public string? EmailAddress { get; set; }

    /// <summary>
    /// User-facing tools Slackbot used on the user's behalf on this date.
    /// </summary>
    public IList<ToolUsage> Tools { get; set; } = [];
}

/// <summary>
/// How many times Slackbot used a tool for a user on a single date.
/// </summary>
public class ToolUsage
{
    /// <summary>
    /// The display name of the tool.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Number of times Slackbot used the tool for the user on this date.
    /// </summary>
    public int UseCount { get; set; }
}

/// <summary>
/// A range of dates, in UTC.
/// </summary>
public class DateRange
{
    /// <summary>
    /// The earliest date (inclusive).
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The latest date (inclusive).
    /// </summary>
    public DateTime EndDate { get; set; }
}
