using System;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A user's assignment to an admin role for an entity.
/// </summary>
public class RoleAssignment
{
    /// <summary>
    /// ID of the role that is assigned.
    /// </summary>
    public required string RoleId { get; set; }

    /// <summary>
    /// ID of entity for which the role is assigned.
    /// </summary>
    public required string EntityId { get; set; }

    /// <summary>
    /// The <see cref="User.Id"/> of the user for which the role is assigned.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Timestamp of when the membership was created.
    /// </summary>
    public int DateCreate { get; set; }

    /// <summary>
    /// When the membership was created.
    /// </summary>
    [JsonIgnore]
    public DateTime Created => DateCreate.ToDateTime().GetValueOrDefault();
}
