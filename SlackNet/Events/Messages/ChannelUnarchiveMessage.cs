namespace SlackNet.Events;

/// <summary>
/// Sent when a channel is unarchived.
/// </summary>
[SlackType("channel_unarchive")]
public class ChannelUnarchiveMessage : MessageEvent { }