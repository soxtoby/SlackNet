#nullable enable
namespace SlackNet;

/// <summary>
/// An external user invited to a shared channel.
/// </summary>
public class InviteTargetUser
{
    /// <summary>
    /// The ID of the invite, used to approve or deny the request with <see cref="WebApi.IConversationsApi.ApproveRequestedShareInvite"/> or <see cref="WebApi.IConversationsApi.DenyRequestedShareInvite"/>.
    /// </summary>
    public required string InviteId { get; set; }

    /// <summary>
    /// The email of the invited user.
    /// </summary>
    public required string Email { get; set; }
}
