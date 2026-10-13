using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Displays a single task, which represents a single action: a title, a status indicator and optional details, output and sources.
/// Use a <see cref="PlanBlock"/> to show several related tasks together.
/// Slack doesn't allow a message to contain both a plan block and task cards.
/// When streaming, the equivalent is a <see cref="WebApi.TaskUpdateChunk"/>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/task-card-block/">Slack documentation</a> for more information.</remarks>
[SlackType("task_card")]
public class TaskCardBlock() : Block("task_card")
{
    /// <summary>
    /// ID for the task. Within a <see cref="PlanBlock"/> each task ID must be unique.
    /// </summary>
    public string TaskId { get; set; }

    /// <summary>
    /// Title of the task in plain text.
    /// </summary>
    public string Title { get; set; }

    public TaskCardStatus Status { get; set; }

    /// <summary>
    /// Details of the task.
    /// </summary>
    public RichTextBlock Details { get; set; }

    /// <summary>
    /// Output of the task.
    /// </summary>
    public RichTextBlock Output { get; set; }

    /// <summary>
    /// URLs that were used to generate a response.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<UrlSourceElement> Sources { get; set; } = [];

    /// <summary>
    /// When true, <see cref="Title"/> is not displayed and <see cref="Details"/> becomes the top element. <see cref="Title"/> is still required.
    /// </summary>
    public bool? HideTitle { get; set; }

    /// <summary>
    /// Icon associated with the task.
    /// </summary>
    public TaskCardIcon Icon { get; set; }
}

/// <summary>
/// One of Slack's named task icons.
/// </summary>
public class TaskCardIcon
{
    public string Type { get; set; } = "icon";

    /// <summary>
    /// The name of the icon, e.g. <c>rocket</c>, <c>bug</c>, <c>globe</c> or <c>check</c>. Slack only accepts the names it defines, and rejects the message with <c>invalid_blocks</c> for any other.
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// The state of a task.
/// </summary>
public enum TaskCardStatus
{
    /// <summary>
    /// The task hasn't started. Slack accepts this for the tasks of a <see cref="PlanBlock"/> and in streamed task updates,
    /// but rejects it for a <see cref="TaskCardBlock"/> that isn't part of a plan.
    /// </summary>
    Pending,
    InProgress,
    Complete,
    Error
}