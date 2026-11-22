#nullable disable
namespace SlackNet.Events;

/// <summary>
/// User requested an invite.
/// When an app subscribed to this event is installed to an org, workspace owners and admins will no longer receive invite notifications from Slack.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/invite_requested/">Slack documentation</a> for more information.</remarks>
public class InviteRequested : Event
{
    public InviteRequest InviteRequest { get; set; }
}
