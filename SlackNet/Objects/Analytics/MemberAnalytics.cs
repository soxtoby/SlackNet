#nullable enable
using System;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// Analytics for a member of an Enterprise organization on a single date.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.getFile/#member_analytics_fields">Slack documentation</a> for more information.</remarks>
public class MemberAnalytics
{
    /// <summary>
    /// The date this row of data is representative of.
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
    /// </summary>
    public string? EmailAddress { get; set; }

    /// <summary>
    /// This field is pulled from data synced via a <a href="https://docs.slack.dev/reference/scim-api/#user-attributes">SCIM API custom attribute</a>.
    /// </summary>
    public string? EnterpriseEmployeeNumber { get; set; }

    /// <summary>
    /// User is classified as a guest (not a full workspace member) on the date in the API request.
    /// </summary>
    public bool IsGuest { get; set; }

    /// <summary>
    /// User is classified as a billable monthly active user (<a href="https://slack.com/help/articles/218915077-Fair-Billing-Policy">included in the bill</a>)
    /// for invoiced customers on the date in the API request.
    /// Customers who purchased their plan via the Slack website and pay for it by
    /// <a href="https://slack.com/help/articles/360002038947-Supported-payment-methods#pay-by-invoice">credit card or self-serve invoicing</a>
    /// should not use this to count their billable users.
    /// </summary>
    public bool IsBillableSeat { get; set; }

    /// <summary>
    /// User has posted a message or read at least one channel or direct message on the date in the API request.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// User has posted a message or read at least one channel or direct message on the date in the API request via the Slack iOS app.
    /// </summary>
    public bool IsActiveIos { get; set; }

    /// <summary>
    /// User has posted a message or read at least one channel or direct message on the date in the API request via the Slack Android app.
    /// </summary>
    public bool IsActiveAndroid { get; set; }

    /// <summary>
    /// User has posted a message or read at least one channel or direct message on the date in the API request via the Slack desktop app.
    /// </summary>
    public bool IsActiveDesktop { get; set; }

    /// <summary>
    /// Total reactions added to any message type in any conversation type by the user on the date in the API request.
    /// Removing reactions is not included.
    /// This metric is not de-duplicated by message; if a user adds 3 different reactions to a single message, 3 reactions are reported.
    /// </summary>
    public int ReactionsAddedCount { get; set; }

    /// <summary>
    /// Total messages posted by the user on the date in the API request to all message and conversation types,
    /// whether public, private, multi-person direct message, etc.
    /// </summary>
    public int MessagesPostedCount { get; set; }

    /// <summary>
    /// Total messages posted by the user in private channels and public channels on the date in the API request, not including direct messages.
    /// </summary>
    public int ChannelMessagesPostedCount { get; set; }

    /// <summary>
    /// Total files uploaded by the user on the date in the API request.
    /// </summary>
    public int FilesAddedCount { get; set; }

    /// <summary>
    /// True when this member has interacted with a Slack app or custom integration on the given day,
    /// or if such an app or integration has performed an action on the user's behalf, such as updating their custom status.
    /// </summary>
    public bool IsActiveApps { get; set; }

    /// <summary>
    /// True when this member has interacted with at least one workflow on a given day.
    /// </summary>
    public bool IsActiveWorkflows { get; set; }

    /// <summary>
    /// True when the member is considered active on Slack Connect,
    /// in that they've read or posted a message to a channel or direct message shared with at least one external workspace.
    /// </summary>
    public bool IsActiveSlackConnect { get; set; }

    /// <summary>
    /// Total number of calls made or joined on a given day, including huddles, Slack native calls,
    /// and calls made using third-party software besides Slack calls.
    /// </summary>
    public int TotalCallsCount { get; set; }

    /// <summary>
    /// Total number of Slack calls made or joined on a given day, excluding any calls using third-party software besides Slack calls.
    /// </summary>
    public int SlackCallsCount { get; set; }

    /// <summary>
    /// Total number of Slack huddles made or joined on a given day.
    /// </summary>
    public int SlackHuddlesCount { get; set; }

    /// <summary>
    /// The number of searches this user performed in Slack on a given day.
    /// </summary>
    public int SearchCount { get; set; }

    /// <summary>
    /// The date of the very first time the member signed in to any workspace within the Enterprise organization, presented in seconds since the epoch (UNIX time).
    /// </summary>
    public int DateClaimed { get; set; }

    /// <summary>
    /// The date of the very first time the member signed in to any workspace within the Enterprise organization.
    /// </summary>
    [JsonIgnore]
    public DateTime Claimed => DateClaimed.ToDateTime().GetValueOrDefault();
}
