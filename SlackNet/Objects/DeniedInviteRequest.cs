#nullable enable
namespace SlackNet;

/// <summary>
/// A request for a person to be invited to a workspace, which has been denied.
/// </summary>
public class DeniedInviteRequest
{
    public required InviteRequest InviteRequest { get; set; }

    /// <summary>
    /// Who denied the request.
    /// </summary>
    public required Actor DeniedBy { get; set; }
}
