#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// A workflow, as returned by <see cref="WebApi.IAdminWorkflowsApi.Search"/>.
/// </summary>
public class Workflow
{
    public required string Id { get; set; }

    /// <summary>
    /// The <see cref="Function.Id"/> of the function that runs the workflow.
    /// </summary>
    public required string WorkflowFunctionId { get; set; }

    public required string CallbackId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    /// <summary>
    /// The workflow's input parameters, keyed by name.
    /// </summary>
    public IDictionary<string, FunctionParameter> InputParameters { get; set; } = new Dictionary<string, FunctionParameter>();

    public IList<WorkflowStep> Steps { get; set; } = [];

    /// <summary>
    /// The <see cref="User.Id"/>s of the workflow's collaborators.
    /// </summary>
    [JsonProperty("collaborators")]
    public IList<string> CollaboratorIds { get; set; } = [];

    public Icons? Icons { get; set; }
    public bool IsPublished { get; set; }

    /// <summary>
    /// The <see cref="User.Id"/> of the user who last updated the workflow.
    /// </summary>
    [JsonProperty("last_updated_by")]
    public string? LastUpdatedByUserId { get; set; }

    public int UnpublishedChangeCount { get; set; }

    /// <summary>
    /// The ID of the workflow's parent app.
    /// </summary>
    public required string AppId { get; set; }

    /// <summary>
    /// Where the workflow was created, such as <c>app</c> or <c>workflow_builder</c>.
    /// </summary>
    public string? Source { get; set; }

    public string? BillingType { get; set; }
    public int DateUpdated { get; set; }
    [JsonIgnore]
    public DateTime Updated => DateUpdated.ToDateTime().GetValueOrDefault();
    public bool IsBillable { get; set; }
    public int CreationSourceType { get; set; }
    public string? CreationSourceId { get; set; }
    public string? LastPublishedVersionId { get; set; }
    public string? LastPublishedDate { get; set; }

    /// <summary>
    /// IDs of the workflow's triggers.
    /// May only include a subset of them, depending on the <c>numTriggerIds</c> passed to <see cref="WebApi.IAdminWorkflowsApi.Search"/>.
    /// </summary>
    public IList<string> TriggerIds { get; set; } = [];

    public bool IsSalesHomeWorkflow { get; set; }
    public bool IsSalesElevate { get; set; }

    /// <summary>
    /// The types of the workflow's triggers.
    /// May only include the types of a subset of them, depending on the <c>numTriggerIds</c> passed to <see cref="WebApi.IAdminWorkflowsApi.Search"/>.
    /// </summary>
    public IList<WorkflowTriggerType> TriggerTypes { get; set; } = [];
}

/// <summary>
/// A step in a <see cref="Workflow"/>.
/// </summary>
public class WorkflowStep
{
    public required string Id { get; set; }

    /// <summary>
    /// The <see cref="Function.Id"/> of the function the step runs.
    /// </summary>
    public required string FunctionId { get; set; }

    /// <summary>
    /// The inputs to the step's function, keyed by input parameter name.
    /// </summary>
    public IDictionary<string, WorkflowStepInput> Inputs { get; set; } = new Dictionary<string, WorkflowStepInput>();
}

/// <summary>
/// An input to a <see cref="WorkflowStep"/>.
/// </summary>
public class WorkflowStepInput
{
    /// <summary>
    /// The value of the input, which may reference workflow inputs or the outputs of earlier steps, such as <c>{{inputs.interactivity}}</c>.
    /// </summary>
    public JToken? Value { get; set; }

    public bool Locked { get; set; }
}

/// <summary>
/// The type of a trigger used by a <see cref="Workflow"/>.
/// </summary>
public class WorkflowTriggerType
{
    /// <summary>
    /// The trigger type ID, which can be used with <see cref="WebApi.IAdminWorkflowsTriggersTypesPermissionsApi"/>.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The type of trigger, such as <c>event</c> or <c>shortcut</c>.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// The trigger's subtype, such as the event for <c>event</c> triggers.
    /// </summary>
    public string? Subtype { get; set; }
}
