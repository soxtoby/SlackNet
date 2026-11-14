#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Who has a permission, such as access to private channels.
/// </summary>
public class EntityPermission
{
    public PermissionVisibility Permission { get; set; }

    /// <summary>
    /// List of user IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// List of team IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> TeamIds { get; set; } = [];

    /// <summary>
    /// List of org IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> OrgIds { get; set; } = [];
}

/// <summary>
/// Who has a permission.
/// </summary>
public enum PermissionVisibility
{
    Everyone,

    /// <summary>
    /// Only the listed users, teams and orgs.
    /// </summary>
    NamedEntities,

    NoOne,

    /// <summary>
    /// The permission hasn't been set.
    /// </summary>
    Unset
}
