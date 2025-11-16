namespace SlackNet.Events;

/// <summary>
/// Sent when a user clicks a Work Object unfurl or refreshes the flexpane.
/// </summary>
public class EntityDetailsRequested : Event
{
    public string User { get; set; }
    public ExternalReference ExternalRef { get; set; }
    public string EntityUrl { get; set; }
    public SharedLink Link { get; set; }
    public string AppUnfurlUrl { get; set; }
    public string TriggerId { get; set; }
    public string UserLocale { get; set; }
    public string Channel { get; set; }
    public string MessageTs { get; set; }
    public string ThreadTs { get; set; }
}
