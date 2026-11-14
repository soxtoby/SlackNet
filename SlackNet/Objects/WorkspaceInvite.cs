#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// An invite for a person to join a workspace.
/// </summary>
public class WorkspaceInvite
{
    public required string Id { get; set; }

    /// <summary>
    /// The <see cref="User.Id"/> of the user who sent the invite.
    /// </summary>
    public required string InviterId { get; set; }

    /// <summary>
    /// The email of the invited person.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The time the invite was sent.
    /// </summary>
    public int DateCreated { get; set; }

    /// <summary>
    /// The time the invite was sent.
    /// </summary>
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The time the invite was last resent.
    /// </summary>
    public int? DateResent { get; set; }

    /// <summary>
    /// The time the invite was last resent.
    /// </summary>
    [JsonIgnore]
    public DateTime? Resent => DateResent?.ToDateTime();

    /// <summary>
    /// Whether emails to the invited person are bouncing.
    /// </summary>
    public bool IsBouncing { get; set; }

    public WorkspaceInvitePreferences InvitePreferences { get; set; } = new();
}

/// <summary>
/// How a person invited to a workspace will join it.
/// </summary>
public class WorkspaceInvitePreferences
{
    /// <summary>
    /// Whether the person will join as a multi-channel guest.
    /// </summary>
    public bool IsRestricted { get; set; }

    /// <summary>
    /// Whether the person will join as a single-channel guest.
    /// </summary>
    public bool IsUltraRestricted { get; set; }

    /// <summary>
    /// The time the invited person's account expires.
    /// </summary>
    public int? DateExpire { get; set; }

    /// <summary>
    /// The time the invited person's account expires.
    /// </summary>
    [JsonIgnore]
    public DateTime? Expires => DateExpire?.ToDateTime();

    /// <summary>
    /// The <see cref="Conversation.Id"/>s of the channels the person will join.
    /// </summary>
    public IList<string> ChannelIds { get; set; } = [];

    public bool IsDomainMatched { get; set; }

    /// <summary>
    /// The name of the invited person.
    /// </summary>
    public string? RealName { get; set; }
}
