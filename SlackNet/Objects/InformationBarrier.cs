#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// An information barrier, which blocks interactions between the members of
/// <a href="https://slack.com/help/articles/115001435788-Connect-identity-provider-groups-to-your-Enterprise-organization">IDP Groups</a>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.barriers.create/">Slack documentation</a> for more information.</remarks>
public class InformationBarrier
{
    public required string Id { get; set; }
    public required string EnterpriseId { get; set; }

    /// <summary>
    /// The primary usergroup, which is barriered from the <see cref="BarrieredFromUsergroups"/>.
    /// Only its <see cref="UserGroup.Id"/> and <see cref="UserGroup.Name"/> are included.
    /// </summary>
    public required UserGroup PrimaryUsergroup { get; set; }

    /// <summary>
    /// The usergroups that the <see cref="PrimaryUsergroup"/> is barriered from.
    /// Only their <see cref="UserGroup.Id"/> and <see cref="UserGroup.Name"/> are included.
    /// </summary>
    public IList<UserGroup> BarrieredFromUsergroups { get; set; } = [];

    /// <summary>
    /// The kinds of interactions that are blocked by this barrier.
    /// </summary>
    public IList<RestrictedSubject> RestrictedSubjects { get; set; } = [];

    /// <summary>
    /// The time the barrier was last updated.
    /// </summary>
    public int DateUpdate { get; set; }

    [JsonIgnore]
    public DateTime Updated => DateUpdate.ToDateTime().GetValueOrDefault();
}

/// <summary>
/// A kind of interaction that can be blocked by an <see cref="InformationBarrier"/>.
/// </summary>
public enum RestrictedSubject
{
    /// <summary>
    /// Direct messages.
    /// </summary>
    Im,

    /// <summary>
    /// Multi-person direct messages.
    /// </summary>
    Mpim,

    /// <summary>
    /// Calls.
    /// </summary>
    Call
}
