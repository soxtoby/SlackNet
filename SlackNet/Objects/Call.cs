using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.WebApi;

namespace SlackNet;

public class Call
{
    /// <summary>
    /// ID associated with the Call.
    /// This ID can be used to post the Call to a channel by using the <see cref="IChatApi.PostMessage"/> method with a <see cref="CallBlock"/> block.
    /// </summary>
    public string Id { get; set; }
    public int DateStart { get; set; }
    public string ExternalUniqueId { get; set; }
    public string JoinUrl { get; set; }
    public string DesktopAppJoinUrl { get; set; }
    public string ExternalDisplayId { get; set; }
    public string Title { get; set; }
    public IList<CallUser> Users { get; set; } = new List<CallUser>();
}