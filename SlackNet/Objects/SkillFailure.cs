#nullable enable
namespace SlackNet;

/// <summary>
/// A skill that could not be published or unpublished.
/// </summary>
public class SkillFailure
{
    /// <summary>
    /// The ID of the skill.
    /// </summary>
    public required string SkillId { get; set; }

    /// <summary>
    /// The reason the skill failed: <c>skill_not_found</c>, <c>method_not_supported_for_skill_type</c>, <c>restricted_action</c> or <c>internal_error</c>.
    /// Missing, deleted, non-skill, and other organizations' files all return <c>skill_not_found</c>.
    /// </summary>
    public required string Error { get; set; }
}
