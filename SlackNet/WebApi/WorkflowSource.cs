#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// Source of workflow creation, for filtering the workflows found by <see cref="IAdminWorkflowsApi.Search"/>.
/// </summary>
public enum WorkflowSource
{
    Code,
    WorkflowBuilder,
    SalesHome,
    Agent
}
