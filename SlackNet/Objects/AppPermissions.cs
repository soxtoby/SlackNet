using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Who can access an app, and where it can be used.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.managed.permissions.set/">Slack documentation</a> for more information.</remarks>
public class AppPermissions
{
    /// <summary>
    /// The type of permission that defines who can access the app, usually one of the <see cref="AppPermissionType"/> values in snake_case.
    /// </summary>
    /// <remarks>
    /// Slack doesn't document how <see cref="WebApi.ManagedAppPermissionType.AppOwner"/> is reported here,
    /// so this is a string rather than an enum, to avoid failing on values SlackNet doesn't know about.
    /// </remarks>
    public required string PermissionType { get; set; }

    /// <summary>
    /// The users that the <see cref="PermissionType"/> applies to,
    /// for <see cref="AppPermissionType.NamedEntities"/> or <see cref="AppPermissionType.NamedEntitiesExclude"/>.
    /// </summary>
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// The usergroups that the <see cref="PermissionType"/> applies to,
    /// for <see cref="AppPermissionType.NamedEntities"/> or <see cref="AppPermissionType.NamedEntitiesExclude"/>.
    /// </summary>
    public IList<string> UsergroupIds { get; set; } = [];

    /// <summary>
    /// The mode that defines where the app can be used in channels.
    /// Only included when channel restrictions are configured.
    /// </summary>
    public ChannelRestrictionMode? ChannelRestrictionMode { get; set; }

    /// <summary>
    /// The channel IDs that the <see cref="ChannelRestrictionMode"/> applies to,
    /// for <see cref="SlackNet.ChannelRestrictionMode.SpecificChannels"/> or <see cref="SlackNet.ChannelRestrictionMode.AllChannelsExcept"/>.
    /// </summary>
    public IList<string> ChannelIds { get; set; } = [];
}

/// <summary>
/// Who can access an app or MCP server.
/// </summary>
public enum AppPermissionType
{
    /// <summary>
    /// Everyone in the organization.
    /// </summary>
    Everyone,

    /// <summary>
    /// Only the listed users and usergroups.
    /// </summary>
    NamedEntities,

    /// <summary>
    /// Everyone except the listed users and usergroups.
    /// </summary>
    NamedEntitiesExclude,

    /// <summary>
    /// No one.
    /// </summary>
    NoOne
}

/// <summary>
/// Where an app can be used in channels.
/// </summary>
public enum ChannelRestrictionMode
{
    /// <summary>
    /// The app can be used in any channel.
    /// </summary>
    AllChannels,

    /// <summary>
    /// The app can only be used in the channels listed in <see cref="AppPermissions.ChannelIds"/> (an allowlist).
    /// </summary>
    SpecificChannels,

    /// <summary>
    /// The app can be used in every channel except those listed in <see cref="AppPermissions.ChannelIds"/> (an exclusion list).
    /// </summary>
    AllChannelsExcept,

    /// <summary>
    /// The app can't be used in any channel, because its permission type is <c>no_one</c>.
    /// </summary>
    NoOne
}
