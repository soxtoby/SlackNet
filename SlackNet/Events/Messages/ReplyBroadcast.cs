using System;

namespace SlackNet.Events;

/// <summary>
/// Sent when a user or bot user has indicated their reply should be broadcast to the whole channel.
/// </summary>
[Obsolete("Use ThreadBroadcast instead.")]
public class ReplyBroadcast : MessageEvent;