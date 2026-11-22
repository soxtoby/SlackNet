#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// Published status, for filtering the workflows found by <see cref="IAdminWorkflowsApi.Search"/>
/// or the skills listed by <see cref="IAdminSlackbotSkillsApi.List"/>.
/// </summary>
public enum PublishStatus
{
    All,
    Published,
    Unpublished
}
