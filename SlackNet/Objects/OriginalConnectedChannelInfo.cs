#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// A disconnected channel, with the original channel ID used for its Enterprise Key Management encryption keys,
/// as returned by <see cref="WebApi.IAdminConversationsEkmApi.ListOriginalConnectedChannelInfo"/>.
/// </summary>
public class OriginalConnectedChannelInfo
{
    public required string Id { get; set; }
    public IList<string> InternalTeamIds { get; set; } = [];
    public required string OriginalConnectedHostId { get; set; }
    public required string OriginalConnectedChannelId { get; set; }
}
