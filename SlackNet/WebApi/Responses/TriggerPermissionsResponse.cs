using System.Collections.Generic;

namespace SlackNet.WebApi;

public class TriggerPermissionsResponse : SlackObject
{
    public TriggerPermissionType PermissionType { get; set; }
    public IList<string> UserIds { get; set; } = [];
    public IList<string> ChannelIds { get; set; } = [];
    public IList<string> TeamIds { get; set; } = [];
    public IList<string> OrgIds { get; set; } = [];
}
