#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A request for a person to be invited to a workspace.
/// </summary>
public class InviteRequest
{
    public required string Id { get; set; }

    /// <summary>
    /// The email of the new, invited user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The time the invite was requested.
    /// </summary>
    public int DateCreated { get; set; }

    /// <summary>
    /// The time the invite was requested.
    /// </summary>
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The <see cref="User.Id"/>s of the users who requested the invite.
    /// </summary>
    public IList<string> RequesterIds { get; set; } = [];

    /// <summary>
    /// The <see cref="Conversation.Id"/>s of the channels to invite the person to.
    /// </summary>
    public IList<string> ChannelIds { get; set; } = [];

    /// <summary>
    /// Whether the user is a multi-channel guest, a single-channel guest, or full member.
    /// Not included in <see cref="WebApi.IAdminInviteRequestsDeniedApi.List"/> responses, which use <see cref="IsRestricted"/> and <see cref="IsUltraRestricted"/> instead.
    /// </summary>
    public InviteRequestType? InviteType { get; set; }

    /// <summary>
    /// Whether the user is a multi-channel guest.
    /// Only included in <see cref="WebApi.IAdminInviteRequestsDeniedApi.List"/> responses; otherwise, see <see cref="InviteType"/>.
    /// </summary>
    public bool? IsRestricted { get; set; }

    /// <summary>
    /// Whether the user is a single-channel guest.
    /// Only included in <see cref="WebApi.IAdminInviteRequestsDeniedApi.List"/> responses; otherwise, see <see cref="InviteType"/>.
    /// </summary>
    public bool? IsUltraRestricted { get; set; }

    /// <summary>
    /// The name of the person to invite.
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// The time the invite request expires.
    /// </summary>
    public int? DateExpire { get; set; }

    /// <summary>
    /// The time the invite request expires.
    /// </summary>
    [JsonIgnore]
    public DateTime? Expires => DateExpire?.ToDateTime();

    /// <summary>
    /// The reason given for inviting the person.
    /// </summary>
    public string? RequestReason { get; set; }

    /// <summary>
    /// The workspace the person will be invited to.
    /// Only included in the <see cref="Events.InviteRequested"/> event.
    /// </summary>
    public Team? Team { get; set; }
}

/// <summary>
/// The type of member a user is invited as.
/// </summary>
public enum InviteRequestType
{
    /// <summary>
    /// Multi-channel guest.
    /// </summary>
    Restricted,

    /// <summary>
    /// Single-channel guest.
    /// </summary>
    UltraRestricted,

    /// <summary>
    /// Full member.
    /// </summary>
    FullMember
}
