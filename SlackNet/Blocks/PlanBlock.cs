using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Groups related <see cref="TaskCardBlock"/>s under a single title, so users can follow a multi-step process as one list.
/// Each task in a plan must have a unique <see cref="TaskCardBlock.TaskId"/>, and a plan can contain at most 50 tasks.
/// Slack doesn't allow a message to contain both a plan block and <see cref="TaskCardBlock"/>s.
/// When streaming, set <see cref="WebApi.TaskDisplayMode.Plan"/> on <see cref="WebApi.IChatApi.StartStream"/> to collect task updates into a plan,
/// and send <see cref="WebApi.PlanUpdateChunk"/>s to change its title.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/plan-block/">Slack documentation</a> for more information.</remarks>
[SlackType("plan")]
public class PlanBlock() : Block("plan")
{
    /// <summary>
    /// Title of the plan in plain text.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The tasks of the plan, at most 50.
    /// </summary>
    public IList<TaskCardBlock> Tasks { get; set; } = [];
}