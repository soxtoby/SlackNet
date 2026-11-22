#nullable disable
using System.Collections.Generic;

namespace SlackNet.Events;

/// <summary>
/// A shared channel invite was requested.
/// A customer must opt in to receive this event. When enabled, external user invitations will not be sent until approved.
/// To approve an invitation, use <see cref="WebApi.IConversationsApi.ApproveRequestedShareInvite"/>.
/// To deny an invitation, use <see cref="WebApi.IConversationsApi.DenyRequestedShareInvite"/>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/shared_channel_invite_requested/">Slack documentation</a> for more information.</remarks>
public class SharedChannelInviteRequested : Event
{
    /// <summary>
    /// The channel the external users were invited to.
    /// </summary>
    public InviteChannel Channel { get; set; }

    /// <summary>
    /// The user who invited the external users.
    /// </summary>
    public User Actor { get; set; }

    /// <summary>
    /// Whether the invited users will have post-only permissions in the channel.
    /// </summary>
    public bool IsExternalLimited { get; set; }

    /// <summary>
    /// The external users who were invited.
    /// </summary>
    public IList<InviteTargetUser> TargetUsers { get; set; } = [];

    /// <summary>
    /// The teams already in the channel.
    /// </summary>
    public IList<ConnectedTeam> TeamsInChannel { get; set; } = [];
}
