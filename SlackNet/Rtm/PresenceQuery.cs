using System.Collections.Generic;

namespace SlackNet.Rtm;

/// <summary>
/// Ask the message server about the current presence status for the specified list of users.
/// </summary>
public class PresenceQuery : OutgoingRtmEvent
{
    /// <summary>
    /// The user IDs you want the presence status for.
    /// </summary>
    public IList<string> Ids { get; set; }
}