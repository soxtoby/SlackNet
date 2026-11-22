using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// Conversation preferences for a public or private channel, as returned by <see cref="WebApi.IAdminConversationsApi.GetConversationPrefs"/>.
/// Preferences that haven't been set for the channel are null.
/// </summary>
public class ConversationPrefs
{
    /// <summary>
    /// Who can post in the channel.
    /// </summary>
    public ConversationPermission? WhoCanPost { get; set; }

    /// <summary>
    /// Who can respond in threads in the channel.
    /// </summary>
    public ConversationPermission? CanThread { get; set; }

    public ConversationMembershipLimit? MembershipLimit { get; set; }

    /// <summary>
    /// Whether a huddle can be started in the channel.
    /// </summary>
    public ConversationPrefToggle? CanHuddle { get; set; }

    /// <summary>
    /// Whether channel mentions are enabled in the channel.
    /// </summary>
    public ConversationPrefToggle? EnableAtChannel { get; set; }

    /// <summary>
    /// Whether here mentions are enabled in the channel.
    /// </summary>
    public ConversationPrefToggle? EnableAtHere { get; set; }
}

/// <summary>
/// The users who have a permission in a channel, such as <see cref="ConversationPrefs.WhoCanPost"/>.
/// </summary>
public class ConversationPermission
{
    /// <summary>
    /// The types of user who have the permission.
    /// </summary>
    [JsonProperty("type")]
    public IList<ConversationPermissionType> Types { get; set; } = [];

    /// <summary>
    /// Specific users who have the permission.
    /// </summary>
    [JsonProperty("user")]
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// Usergroups whose members have the permission.
    /// </summary>
    [JsonProperty("subteam")]
    public IList<string> UsergroupIds { get; set; } = [];
}

/// <summary>
/// A type of user who can have a <see cref="ConversationPermission"/>.
/// </summary>
public enum ConversationPermissionType
{
    /// <summary>
    /// Everyone. In Slack Connect channels, all home and away workspace members and guests.
    /// </summary>
    Ra,

    /// <summary>
    /// In Slack Connect channels, only home workspace members, but not away workspace members or home or away workspace guests.
    /// </summary>
    Regular,

    /// <summary>
    /// Admins. In Slack Connect channels, only home admins.
    /// </summary>
    Admin,

    /// <summary>
    /// Org admins. Can only be used if the channel is an org-wide channel.
    /// In Slack Connect channels, only home org admins.
    /// </summary>
    OrgAdmin,

    Owner,

    /// <summary>
    /// Any user. Only honored when <see cref="Admin"/> or <see cref="OrgAdmin"/> is also provided.
    /// </summary>
    User
}

public class ConversationMembershipLimit
{
    /// <summary>
    /// Membership limit value.
    /// </summary>
    public int Value { get; set; }
}

public class ConversationPrefToggle
{
    public bool Enabled { get; set; }
}
