using System.Collections.Generic;

namespace SlackNet.Events;

/// <summary>
/// Sent when a private group is archived.
/// </summary>
[SlackType("group_archive")]
public class GroupArchiveMessage : MessageEvent
{
    public IList<string> Members { get; set; } = [];
}