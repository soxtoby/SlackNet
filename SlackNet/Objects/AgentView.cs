using System;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A view attached to a code channel, rendered as a tab in the channel.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.listViews/">Slack documentation</a> for more information.</remarks>
public class AgentView
{
    /// <summary>
    /// The encoded channel tab ID of the view.
    /// </summary>
    public required string ViewId { get; set; }

    /// <summary>
    /// The agent-assigned stable key of the view. Absent for diff views.
    /// </summary>
    public string? ViewKey { get; set; }

    /// <summary>
    /// The encoded ID of the backing file.
    /// </summary>
    public required string FileId { get; set; }

    /// <summary>
    /// The display label of the view tab.
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// The current content version of the view.
    /// </summary>
    public int ContentVersion { get; set; }

    /// <summary>
    /// Unix timestamp of when the view was first created.
    /// </summary>
    public int DateAdded { get; set; }

    /// <summary>
    /// The time the view was first created.
    /// </summary>
    [JsonIgnore]
    public DateTime Added => DateAdded.ToDateTime().GetValueOrDefault();
}

/// <summary>
/// The kind of an <see cref="AgentView"/>, which determines what it renders.
/// </summary>
public enum AgentViewType
{
    /// <summary>
    /// A full, self-contained HTML document, rendered in a sandboxed context.
    /// </summary>
    Html,

    /// <summary>
    /// A unified diff, shown in the channel's code tab.
    /// </summary>
    Diff,

    /// <summary>
    /// Block Kit blocks.
    /// </summary>
    BlockKit,

    /// <summary>
    /// A canvas.
    /// </summary>
    Canvas
}
