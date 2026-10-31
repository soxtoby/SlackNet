namespace SlackNet.WebApi;

public class AcceptSharedInviteResponse : SlackObject
{
    public bool ImplicitApproval { get; set; }
    public string ChannelId { get; set; }
    public string InviteId { get; set; }
}