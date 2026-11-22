using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// Analytics for activity in a single public channel on a single date.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.getFile/#conversation_analytics_fields">Slack documentation</a> for more information.</remarks>
public class PublicChannelAnalytics
{
    /// <summary>
    /// Immutable, unique ID for the Enterprise organization.
    /// When the API is called by a customer not in an Enterprise organization, this field has value "0".
    /// </summary>
    public required string EnterpriseId { get; set; }

    /// <summary>
    /// Immutable, unique ID of the team.
    /// </summary>
    public required string TeamId { get; set; }

    /// <summary>
    /// The workspace that created this public channel.
    /// </summary>
    public required AnalyticsTeam OriginatingTeam { get; set; }

    /// <summary>
    /// The unique ID belonging to this channel.
    /// <see cref="WebApi.IAdminAnalyticsApi.GetPublicChannelMetadata"/> will give you information like the channel's name.
    /// </summary>
    public required string ChannelId { get; set; }

    /// <summary>
    /// Indicates which kind of public channel this is.
    /// </summary>
    public AnalyticsChannelType ChannelType { get; set; }

    /// <summary>
    /// Indicates whether the channel is public or private. Only public channel analytics is available at this time.
    /// </summary>
    public AnalyticsChannelVisibility Visibility { get; set; }

    /// <summary>
    /// Indicates which, if any, workspaces in the same organization this channel is shared with.
    /// Only works with <see cref="ChannelType"/> set to <see cref="AnalyticsChannelType.MultiWorkspaceChannel"/>.
    /// One of the included <see cref="AnalyticsTeam.TeamId"/> values corresponds to the organization itself.
    /// </summary>
    public IList<AnalyticsTeam> SharedWith { get; set; } = [];

    /// <summary>
    /// Whether the channel is shared with workspaces outside of this organization.
    /// </summary>
    public bool IsSharedExternally { get; set; }

    /// <summary>
    /// Indicates which, if any, external organizations this channel is shared with.
    /// Only works with <see cref="IsSharedExternally"/> set to true.
    /// </summary>
    public IList<AnalyticsOrganization> ExternallySharedWithOrganizations { get; set; } = [];

    /// <summary>
    /// The date and time the channel was first created, presented in seconds since the epoch (UNIX time).
    /// </summary>
    public int DateCreated { get; set; }

    /// <summary>
    /// The date and time the channel was first created.
    /// </summary>
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The date and time the channel last had a message posted in it, presented in seconds since the epoch (UNIX time).
    /// </summary>
    public int DateLastActive { get; set; }

    /// <summary>
    /// The date and time the channel last had a message posted in it.
    /// </summary>
    [JsonIgnore]
    public DateTime? LastActive => DateLastActive.ToDateTime();

    /// <summary>
    /// A count of total full members and guests.
    /// </summary>
    public int TotalMembersCount { get; set; }

    /// <summary>
    /// A count of people in this channel who have a full member account.
    /// </summary>
    public int FullMembersCount { get; set; }

    /// <summary>
    /// A count of people in this channel who have a guest account.
    /// </summary>
    public int GuestMemberCount { get; set; }

    /// <summary>
    /// A count of total messages posted, including messages from apps and integrations on the given day.
    /// </summary>
    public int MessagesPostedCount { get; set; }

    /// <summary>
    /// A count of total messages posted from Slack members and guests (human users only) on the given day.
    /// </summary>
    public int MessagesPostedByMembersCount { get; set; }

    /// <summary>
    /// A count of the unique number of human users (guests and full members) who posted a message on the given day.
    /// </summary>
    public int MembersWhoPostedCount { get; set; }

    /// <summary>
    /// A count of the unique human users (guests and full members) who read a message on the given day.
    /// </summary>
    public int MembersWhoViewedCount { get; set; }

    /// <summary>
    /// A count of emoji reactions left on any message in the channel on the given day by human users.
    /// </summary>
    public int ReactionsAddedCount { get; set; }

    /// <summary>
    /// The day in question for the query; the date requested by the user for usage metrics.
    /// </summary>
    public DateTime Date { get; set; }
}

/// <summary>
/// A workspace referred to by <see cref="PublicChannelAnalytics"/>.
/// </summary>
public class AnalyticsTeam
{
    public required string TeamId { get; set; }
    public required string Name { get; set; }
}

/// <summary>
/// An external organization that a channel in <see cref="PublicChannelAnalytics"/> is shared with.
/// </summary>
public class AnalyticsOrganization
{
    public required string Name { get; set; }

    /// <summary>
    /// The organization's Slack domain.
    /// </summary>
    public required string Domain { get; set; }
}

/// <summary>
/// Which kind of public channel <see cref="PublicChannelAnalytics"/> is for.
/// </summary>
public enum AnalyticsChannelType
{
    SingleWorkspaceChannel,
    MultiWorkspaceChannel,
    OrgWideChannel
}

/// <summary>
/// Whether the channel <see cref="PublicChannelAnalytics"/> is for is public or private.
/// </summary>
public enum AnalyticsChannelVisibility
{
    Public,
    Private
}
