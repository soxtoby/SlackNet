#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A public or private channel in an Enterprise organization, as found by <see cref="WebApi.IAdminConversationsApi.Search"/>.
/// </summary>
public class AdminConversation
{
    public required string Id { get; set; }

    /// <summary>
    /// The name of the channel, without a leading hash sign.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The channel's purpose.
    /// </summary>
    public string Purpose { get; set; } = string.Empty;

    /// <summary>
    /// The number of members in the channel. Will be -1 when the channel is archived.
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// Unix timestamp of when the channel was created.
    /// </summary>
    public int Created { get; set; }

    [JsonIgnore]
    public DateTime CreatedDate => Created.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The user ID of the member that created this channel.
    /// </summary>
    public required string CreatorId { get; set; }

    /// <summary>
    /// Means the channel is privileged between two or more members.
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Indicates a channel is archived. Frozen in time.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Means the channel is the workspace's "general" discussion channel (even if it's not named #general but it might be anyway).
    /// </summary>
    public bool IsGeneral { get; set; }

    /// <summary>
    /// When there was last activity in the channel, in epoch microseconds.
    /// </summary>
    public long LastActivityTs { get; set; }

    [JsonIgnore]
    public DateTime LastActivity => (LastActivityTs / 1_000_000m).ToDateTime().GetValueOrDefault();

    /// <summary>
    /// Represents this channel as being part of a Shared Channel with a remote organization.
    /// </summary>
    public bool IsExtShared { get; set; }

    public bool IsGlobalShared { get; set; }

    public bool IsOrgDefault { get; set; }

    public bool IsOrgMandatory { get; set; }

    /// <summary>
    /// Explains whether this shared channel is shared between Enterprise Grid workspaces within the same organization.
    /// </summary>
    public bool IsOrgShared { get; set; }

    /// <summary>
    /// Whether the channel is frozen (e.g. a disconnected Slack Connect channel becomes a read-only frozen copy on the away-team side).
    /// </summary>
    public bool IsFrozen { get; set; }

    /// <summary>
    /// Connected external team IDs.
    /// </summary>
    public IList<string> ConnectedTeamIds { get; set; } = [];

    /// <summary>
    /// The number of workspaces in the organization that the channel is connected to.
    /// </summary>
    public int InternalTeamIdsCount { get; set; }

    /// <summary>
    /// The ID of one of the workspaces in the organization that the channel is connected to.
    /// </summary>
    [JsonProperty("internal_team_ids_sample_team")]
    public string? SampleInternalTeamId { get; set; }

    /// <summary>
    /// Pending connected team IDs.
    /// </summary>
    public IList<string> PendingConnectedTeamIds { get; set; } = [];

    /// <summary>
    /// Means the channel is ready to become an <see cref="IsExtShared"/> channel but isn't quite ready yet and needs some kind of approval or sign off.
    /// </summary>
    public bool IsPendingExtShared { get; set; }

    public override string ToString() => Link.Conversation(Id).ToString();
}
