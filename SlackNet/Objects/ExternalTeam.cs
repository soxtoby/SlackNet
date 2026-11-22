using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

public class ExternalTeam
{
    public string TeamId { get; set; } = null!;
    public string TeamName { get; set; } = null!;
    public string TeamDomain { get; set; } = null!;
    public int PublicChannelCount { get; set; }
    public int PrivateChannelCount { get; set; }
    public int ImChannelCount { get; set; }
    public int MpimChannelCount { get; set; }
    public ConnectedWorkspace ConnectedWorkspaces { get; set; } = null!;
    public IList<SlackConnectPref> SlackConnectPrefs { get; set; } = null!;
    public string ConnectionStatus { get; set; } = null!;
    public int LastActiveTimestamp { get; set; }
    [JsonIgnore]
    public DateTime? LastActive => LastActiveTimestamp.ToDateTime();
    public bool IsSponsored { get; set; }
    public CanvasConnections? Canvas { get; set; }
    public ListConnections? Lists { get; set; }
}

public class ConnectedWorkspace
{
    public string? WorkspaceId { get; set; }
    public string? WorkspaceName { get; set; }
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
    public IList<OwnershipDetails> OwnershipDetails { get; set; } = null!;
}

public class ListConnections
{
    public int TotalCount { get; set; }
    public IList<OwnershipDetails> OwnershipDetails { get; set; } = null!;
}

public class OwnershipDetails
{
    public string TeamId { get; set; } = null!;
    public int? Count { get; set; }
}
