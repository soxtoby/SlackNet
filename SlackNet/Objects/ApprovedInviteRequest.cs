namespace SlackNet;

/// <summary>
/// A request for a person to be invited to a workspace, which has been approved.
/// </summary>
public class ApprovedInviteRequest
{
    public required InviteRequest InviteRequest { get; set; }

    /// <summary>
    /// Who approved the request.
    /// </summary>
    public required Actor ApprovedBy { get; set; }

    /// <summary>
    /// The invite sent when the request was approved.
    /// </summary>
    public required WorkspaceInvite Invite { get; set; }
}
