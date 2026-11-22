namespace SlackNet.WebApi;

public class AcceptSharedInviteResponse
{
    public bool ImplicitApproval { get; set; }
    public string ChannelId { get; set; } = null!;
    public string InviteId { get; set; } = null!;
}