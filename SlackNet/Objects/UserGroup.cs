using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

public class UserGroup
{
    public string Id { get; set; }
    public string TeamId { get; set; }
    public bool IsUsergroup { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Handle { get; set; }
    public bool IsExternal { get; set; }
    public int DateCreate { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreate.ToDateTime().GetValueOrDefault();
    public int DateUpdate { get; set; }
    [JsonIgnore]
    public DateTime? Updated => DateUpdate.ToDateTime();
    public int DateDelete { get; set; }
    [JsonIgnore]
    public DateTime? Deleted => DateDelete.ToDateTime();
    public UserGroupAutoType? AutoType { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string DeletedBy { get; set; }
    public UserGroupPreferences Prefs { get; set; }
    public IList<string> Users { get; set; } = [];
    public int UserCount { get; set; }

    /// <summary>
    /// Enterprise organization ID this usergroup belongs to.
    /// </summary>
    public string EnterpriseId { get; set; }

    /// <summary>
    /// Enterprise-level subteam ID.
    /// </summary>
    public string EnterpriseSubteamId { get; set; }

    /// <summary>
    /// Whether this is a subteam.
    /// </summary>
    public bool IsSubteam { get; set; }

    /// <summary>
    /// Whether members are auto-provisioned into this group.
    /// </summary>
    public bool AutoProvision { get; set; }

    /// <summary>
    /// Whether this usergroup represents a sidebar section.
    /// </summary>
    public bool IsSection { get; set; }

    /// <summary>
    /// Whether editing the usergroup is restricted.
    /// </summary>
    public bool IsEditingRestricted { get; set; }

    /// <summary>
    /// Whether membership changes are locked.
    /// </summary>
    public bool IsMembershipLocked { get; set; }

    /// <summary>
    /// Whether this usergroup is synced from an identity provider (SCIM/SSO).
    /// </summary>
    public bool IsIdpGroup { get; set; }

    /// <summary>
    /// Whether the usergroup is visible to members.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Whether this is an org-level (enterprise-wide) usergroup.
    /// </summary>
    public bool IsOrgLevel { get; set; }

    /// <summary>
    /// Workspace IDs this usergroup spans (for org-level groups).
    /// </summary>
    public IList<string> Teams { get; set; } = [];

    /// <summary>
    /// Number of channels associated with this usergroup.
    /// </summary>
    public int ChannelCount { get; set; }
}