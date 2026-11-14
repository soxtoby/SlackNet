#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// <b>Legacy feature</b>: settings that describe a workflow step. This feature has been deprecated.
/// </summary>
public class WorkflowStep
{
    /// <summary>
    /// The name of the workflow step.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The <c>callback_id</c> of the workflow step.
    /// </summary>
    public required string CallbackId { get; set; }
}
