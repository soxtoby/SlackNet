using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A user in an Enterprise organization, as listed by <see cref="WebApi.IAdminUsersApi.List"/>.
/// </summary>
/// <remarks>
/// <see cref="WebApi.IAdminUsersApi.GetExpiration"/> only returns
/// <see cref="Id"/>, <see cref="Username"/>, <see cref="Email"/>, <see cref="IsRestricted"/>, <see cref="IsUltraRestricted"/> and <see cref="ExpirationTs"/>.
/// </remarks>
public class AdminUser
{
    /// <summary>
    /// Encoded user ID.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Username of user.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Full name of user.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// If 2FA is enabled. Only returned with org token and no team ID.
    /// </summary>
    [JsonProperty("has_2fa")]
    public bool? Has2FA { get; set; }

    /// <summary>
    /// If SSO is enabled. Only returned with org token and no team ID.
    /// </summary>
    public bool? HasSso { get; set; }

    /// <summary>
    /// Email address of user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Is this user an admin?
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Is this user an owner?
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Is this user the primary owner of the workspace?
    /// </summary>
    public bool IsPrimaryOwner { get; set; }

    /// <summary>
    /// Is this user a restricted (multi-channel guest) user?
    /// </summary>
    public bool IsRestricted { get; set; }

    /// <summary>
    /// Is this user an ultra-restricted (single-channel guest) user?
    /// </summary>
    public bool IsUltraRestricted { get; set; }

    /// <summary>
    /// Is this user a bot user?
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// For a guest user, what is their expiration timestamp? (On workspaces only.)
    /// </summary>
    public long ExpirationTs { get; set; }

    [JsonIgnore]
    public DateTime? Expiration => ExpirationTs.ToDateTime();

    /// <summary>
    /// Roles that a user has.
    /// </summary>
    public IList<string> Roles { get; set; } = [];

    /// <summary>
    /// Last active timestamp for a user. Only applies with org token and no team ID.
    /// </summary>
    public int LastActiveTs { get; set; }

    [JsonIgnore]
    public DateTime? LastActive => LastActiveTs.ToDateTime();

    /// <summary>
    /// IDs of the workspaces that a user belongs to.
    /// </summary>
    public IList<string> Workspaces { get; set; } = [];

    /// <summary>
    /// Is this user active?
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date of when the user was created.
    /// </summary>
    public int DateCreated { get; set; }

    [JsonIgnore]
    public DateTime? Created => DateCreated.ToDateTime();

    /// <summary>
    /// Date of when the user was deactivated.
    /// </summary>
    public int DeactivatedTs { get; set; }

    [JsonIgnore]
    public DateTime? Deactivated => DeactivatedTs.ToDateTime();
}
