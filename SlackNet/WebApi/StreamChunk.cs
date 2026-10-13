using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet.WebApi;

/// <summary>
/// A piece of a streaming message, sent in the <c>chunks</c> argument of <see cref="IChatApi.StartStream"/>, <see cref="IChatApi.AppendStream"/> and <see cref="IChatApi.StopStream"/>.
/// A stream that was started with chunks must be appended to with chunks, and a stream that was started with markdown text must be appended to with markdown text.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.appendStream/#chunks">Slack documentation</a> for more information.</remarks>
public abstract class StreamChunk(string type)
{
    public string Type { get; set; } = type;
}

/// <summary>
/// Streams text, formatted as standard markdown (not Slack's mrkdwn).
/// </summary>
public class MarkdownTextChunk() : StreamChunk("markdown_text")
{
    /// <summary>
    /// The markdown text to append to the message received so far.
    /// </summary>
    public string Text { get; set; }
}

/// <summary>
/// Creates or updates a task, to display the progress of a unit of work.
/// Tasks are displayed according to the <see cref="TaskDisplayMode"/> of the stream.
/// This is the streaming counterpart of the <see cref="TaskCardBlock"/>.
/// Slack limits the size of <c>task_update</c> and <c>plan_update</c> chunks to 256 characters.
/// </summary>
public class TaskUpdateChunk() : StreamChunk("task_update")
{
    /// <summary>
    /// A unique ID for the task. Sending another update with the same ID updates the task.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Title of the task in plain text.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// When true, <see cref="Title"/> is not displayed and <see cref="Details"/> becomes the top element.
    /// </summary>
    public bool? HideTitle { get; set; }

    /// <summary>
    /// The state of the task. Slack marks tasks that are still <see cref="TaskCardStatus.Pending"/> or <see cref="TaskCardStatus.InProgress"/> when the stream stops as <see cref="TaskCardStatus.Error"/>.
    /// </summary>
    public TaskCardStatus Status { get; set; }

    /// <summary>
    /// Icon associated with the task.
    /// </summary>
    public TaskCardIcon Icon { get; set; }

    /// <summary>
    /// Details of the task.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// Output of the task.
    /// </summary>
    public string Output { get; set; }

    /// <summary>
    /// URLs that were used in the task.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<UrlSourceElement> Sources { get; set; } = [];
}

/// <summary>
/// Updates the title of the plan that tasks are collected into when the stream's <see cref="TaskDisplayMode"/> is <see cref="TaskDisplayMode.Plan"/>.
/// Slack limits the size of <c>task_update</c> and <c>plan_update</c> chunks to 256 characters.
/// </summary>
public class PlanUpdateChunk() : StreamChunk("plan_update")
{
    public string Title { get; set; }
}

/// <summary>
/// Streams blocks as part of the message. At most 50 blocks can be sent in one chunk; Slack drops any extra blocks and returns a warning.
/// </summary>
public class BlocksChunk() : StreamChunk("blocks")
{
    public IList<Block> Blocks { get; set; } = [];
}

/// <summary>
/// How task updates are displayed in a streaming message.
/// </summary>
public enum TaskDisplayMode
{
    /// <summary>
    /// Task updates render as individual task cards, interleaved with the streamed text. This is Slack's default.
    /// </summary>
    Timeline,

    /// <summary>
    /// Task updates render together, in a single plan block.
    /// </summary>
    Plan
}