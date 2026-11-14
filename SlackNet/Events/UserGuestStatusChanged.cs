namespace SlackNet.Events;

/// <summary>
/// Sent when a user's guest status changes, including when a user goes from:
/// <list type="bullet">
///   <item>Regular member to single-channel or multi-channel guest, or the reverse.</item>
///   <item>Single-channel guest to multi-channel guest, or the reverse.</item>
///   <item>Single-channel or multi-channel guest to deactivated guest.</item>
/// </list>
/// To track newly invited users that start out as guests, use the <see cref="TeamJoin"/> event instead.
/// </summary>
public class UserGuestStatusChanged : UserChange;
