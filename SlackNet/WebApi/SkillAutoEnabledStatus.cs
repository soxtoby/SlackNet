#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// Org deployment status, for filtering the skills listed by <see cref="IAdminSlackbotSkillsApi.List"/>.
/// </summary>
public enum SkillAutoEnabledStatus
{
    All,
    Enabled,
    Disabled
}
