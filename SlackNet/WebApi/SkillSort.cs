#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// The field used to sort the skills listed by <see cref="IAdminSlackbotSkillsApi.List"/>.
/// </summary>
public enum SkillSort
{
    /// <summary>
    /// Sort by <see cref="Skill.DisplayName"/>, using case-insensitive ordering.
    /// </summary>
    Name,

    /// <summary>
    /// Sort by <see cref="Skill.UsageCount"/>.
    /// </summary>
    UsageCount,

    /// <summary>
    /// Sort by <see cref="Skill.DateUpdated"/>. Unedited skills use their creation date.
    /// </summary>
    DateUpdated
}
