using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Who a <see cref="Skill"/> is automatically turned on for.
/// This setting does not grant access to its instructions.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.slackbot.skills.setAutoEnabled/">Slack documentation</a> for more information.</remarks>
public class SkillAutoEnabled
{
    /// <summary>
    /// Who receives the skill automatically.
    /// </summary>
    public SkillAutoEnabledPermissionType PermissionType { get; set; }

    /// <summary>
    /// Deployment audience within this organization.
    /// Empty for <see cref="SkillAutoEnabledPermissionType.Everyone"/> or <see cref="SkillAutoEnabledPermissionType.NoOne"/>.
    /// </summary>
    public IList<string> UsergroupIds { get; set; } = [];
}

/// <summary>
/// Who receives a <see cref="Skill"/> automatically.
/// </summary>
public enum SkillAutoEnabledPermissionType
{
    /// <summary>
    /// The skill is turned on for everyone in the organization.
    /// The skill canvas must already be shared with the whole organization.
    /// </summary>
    Everyone,

    /// <summary>
    /// The skill is turned on for the members of the user groups in <see cref="SkillAutoEnabled.UsergroupIds"/>.
    /// Every member of those groups must be able to view the skill canvas.
    /// </summary>
    NamedEntities,

    /// <summary>
    /// Auto-enable is off.
    /// </summary>
    NoOne
}
