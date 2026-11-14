#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A Slackbot skill.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.info/">Slack documentation</a> for more information.</remarks>
public class Skill
{
    /// <summary>
    /// Skill ID: a canvas file ID for canvas skills, or a <c>builtin:</c> reference for built-in skills.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Where the skill is defined.
    /// </summary>
    public SkillType Type { get; set; }

    /// <summary>
    /// Human-readable label. Not unique.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Reserved for a future machine-readable skill name. Always null.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// When Slackbot should use the skill.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Skill category.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Example prompt. <c>{skill}</c> stands for the skill mention.
    /// </summary>
    public string? ExamplePrompt { get; set; }

    /// <summary>
    /// Markdown instructions. Only returned by <see cref="WebApi.ISlackbotSkillsApi.Info"/>; null elsewhere.
    /// </summary>
    public string? Instructions { get; set; }

    /// <summary>
    /// Who in the organization can view the skill. Null for built-in skills.
    /// </summary>
    public SkillVisibility? Visibility { get; set; }

    /// <summary>
    /// Whether the skill is published to the organization's catalog.
    /// </summary>
    public bool IsOfficial { get; set; }

    /// <summary>
    /// The caller's effective access to the skill: <c>read</c>, <c>comment</c>, <c>write</c> or <c>owner</c>.
    /// For canvas skills, the caller's access to the canvas, as in <see cref="File.Access"/>. Built-in skills are always <c>read</c>.
    /// </summary>
    public string? Access { get; set; }

    /// <summary>
    /// Apps and MCP servers the skill needs.
    /// </summary>
    public IList<SkillMcpServer> McpServers { get; set; } = [];

    /// <summary>
    /// Approximate number of runs.
    /// </summary>
    public int? UsageCount { get; set; }

    /// <summary>
    /// Approximate number of people who have the skill enabled.
    /// </summary>
    public int? LibraryCount { get; set; }

    /// <summary>
    /// User ID of the skill's creator. Null for built-in skills.
    /// </summary>
    public string? CreatorId { get; set; }

    /// <summary>
    /// Unix timestamp when the skill was created.
    /// </summary>
    public int? DateCreated { get; set; }

    /// <summary>
    /// When the skill was created.
    /// </summary>
    [JsonIgnore]
    public DateTime? Created => DateCreated?.ToDateTime();

    /// <summary>
    /// Unix timestamp when the skill was last updated.
    /// </summary>
    public int? DateUpdated { get; set; }

    /// <summary>
    /// When the skill was last updated.
    /// </summary>
    [JsonIgnore]
    public DateTime? Updated => DateUpdated?.ToDateTime();

    /// <summary>
    /// Link to the skill.
    /// </summary>
    public string Permalink { get; set; } = null!;
}

/// <summary>
/// Where a <see cref="Skill"/> is defined.
/// </summary>
public enum SkillType
{
    /// <summary>
    /// The skill is defined in a canvas.
    /// </summary>
    Canvas,

    /// <summary>
    /// The skill is built into Slackbot.
    /// </summary>
    Builtin
}

/// <summary>
/// Who in the workspace can view and share a <see cref="Skill"/>.
/// Derived from the highest org or workspace-level access grant on the underlying skill canvas.
/// </summary>
public enum SkillVisibility
{
    /// <summary>
    /// No org or workspace-level grant exists (only the creator and explicitly invited members).
    /// </summary>
    InviteOnly,

    /// <summary>
    /// A read or comment grant exists.
    /// </summary>
    AnyoneCanView,

    /// <summary>
    /// A write grant exists.
    /// </summary>
    AnyoneCanEdit
}
