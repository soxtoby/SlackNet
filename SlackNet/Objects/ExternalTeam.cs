using System.Collections.Generic;

namespace SlackNet;

public class ExternalTeam
{
    public string TeamId { get; set; }
    public string TeamName { get; set; }
    public string TeamDomain { get; set; }
    public int PublicChannelCount { get; set; }
    public int PrivateChannelCount { get; set; }
    public int ImChannelCount { get; set; }
    public int MpimChannelCount { get; set; }
    public ConnectedWorkspace ConnectedWorkspaces { get; set; }
    public IList<SlackConnectPref> SlackConnectPrefs { get; set; }
    public string ConnectionStatus { get; set; }
    public int LastActiveTimestamp { get; set; }
    public bool IsSponsored { get; set; }
    public CanvasConnections Canvas { get; set; }
    public ListConnections Lists { get; set; }
}

public class ConnectedWorkspace
{
    public string WorkspaceId { get; set; }
    public string WorkspaceName { get; set; }
}

public enum SlackConnectPref
{
    ApprovedOrgsOnly,
    AllowScFileUploads, 
    ProfileVisibility, 
    AwayTeamScInvitePermissions,
    AcceptScInvites,
    ScMpdmToPrivate,
    RequireScChannelForScDm,
    ExternalAwarenessContextBar,
}

public class CanvasConnections
{
    public int TotalCount { get; set; }
    public IList<OwnershipDetails> OwnershipDetails { get; set; }
}

public class ListConnections
{
    public int TotalCount { get; set; }
    public IList<OwnershipDetails> OwnershipDetails { get; set; }
}

public class OwnershipDetails
{
    public string TeamId { get; set; }
    public int? Count { get; set; }
}
