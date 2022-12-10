namespace SlackNet.Events;

/// <summary>
/// Used by Slack's web client to maintain a list of logged-in accounts.
/// Other clients should ignore this event.
/// </summary>
public class AccountsChanged : Event { }