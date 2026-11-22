using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// An entity assigned to an authentication policy.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.auth.policy.getEntities/">Slack documentation</a> for more information.</remarks>
public class AuthPolicyEntity
{
    /// <summary>
    /// The type of entity assigned to the policy. Informs the meaning of the <see cref="EntityId"/>.
    /// Null if Slack returned a type SlackNet doesn't know.
    /// </summary>
    public AuthPolicyEntityType? EntityType { get; set; }

    /// <summary>
    /// Encoded ID of the assigned entity. A user ID if the <see cref="EntityType"/> is <see cref="AuthPolicyEntityType.User"/>.
    /// </summary>
    public required string EntityId { get; set; }

    /// <summary>
    /// The date this entity was assigned to this policy.
    /// </summary>
    public int DateAdded { get; set; }

    [JsonIgnore]
    public DateTime Added => DateAdded.ToDateTime().GetValueOrDefault();
}

/// <summary>
/// The type of entity assigned to an authentication policy.
/// </summary>
public enum AuthPolicyEntityType
{
    [EnumMember(Value = "USER")] User
}
