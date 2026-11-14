namespace SlackNet.WebApi;

/// <summary>
/// Published status, for filtering the workflows found by <see cref="IAdminWorkflowsApi.Search"/>.
/// </summary>
public enum WorkflowPublishStatus
{
    All,
    Published,
    Unpublished
}
