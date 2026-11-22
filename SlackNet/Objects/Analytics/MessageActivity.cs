using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Activity metrics for a message, counting unique users.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.messages.activity/">Slack documentation</a> for more information.</remarks>
public class MessageActivity
{
    /// <summary>
    /// ID of the channel the message is in.
    /// </summary>
    public required string ChannelId { get; set; }

    /// <summary>
    /// Timestamp of the message.
    /// </summary>
    public required string Timestamp { get; set; }

    public int UniqueUserReactionsCount { get; set; }
    public int UniqueUserSharesCount { get; set; }
    public int UniqueUserViewsCount { get; set; }
    public int UniqueUserClicksCount { get; set; }

    /// <summary>
    /// Unique users who viewed the message, by the client they viewed it on.
    /// </summary>
    public MessageViewsByClient UniqueViewsClient { get; set; } = new();

    /// <summary>
    /// Unique user activity, by the users' department.
    /// </summary>
    public IList<DepartmentMessageActivity> UniqueStatsByDepartment { get; set; } = [];

    /// <summary>
    /// Unique user activity, by the users' organization or workspace.
    /// </summary>
    public IList<TeamMessageActivity> UniqueStatsByOrg { get; set; } = [];
}

/// <summary>
/// Unique users who viewed a message, by the client they viewed it on.
/// </summary>
public class MessageViewsByClient
{
    public int DesktopViewsCount { get; set; }
    public int MobileViewsCount { get; set; }
    public int WebViewsCount { get; set; }
}

/// <summary>
/// Unique user activity on a message, for a group of users.
/// </summary>
public abstract class GroupMessageActivity
{
    public int Views { get; set; }
    public int Reactions { get; set; }
    public int Shares { get; set; }
    public int Clicks { get; set; }
}

/// <summary>
/// Unique user activity on a message, for users in a department.
/// </summary>
public class DepartmentMessageActivity : GroupMessageActivity
{
    public required string Department { get; set; }
}

/// <summary>
/// Unique user activity on a message, for users in an organization or workspace.
/// </summary>
public class TeamMessageActivity : GroupMessageActivity
{
    public required string TeamId { get; set; }
}
