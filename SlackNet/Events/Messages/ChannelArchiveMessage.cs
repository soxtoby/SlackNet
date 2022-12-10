using System.Collections.Generic;

namespace SlackNet.Events;

/// <summary>
/// Sent when a channel is archived.
/// </summary>
[SlackType("channel_archive")]
public class ChannelArchiveMessage : MessageEvent
{
    public IList<string> Members { get; set; } = new List<string>();
}