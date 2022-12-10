namespace SlackNet.Events;

public class SharedChannelInviteReceived : Event
{
    public Invite Invite { get; set; }
    public InviteChannel Channel { get; set; }
}