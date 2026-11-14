#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Blocks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IAgentsConversationsApi
{
    /// <summary>
    /// Archives a code channel when its session is complete. The channel and its history remain visible in the archived state.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.archive/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of the code channel to archive.</param>
    /// <param name="summaryMessageTs">
    /// Timestamp of a message in the code channel to share back as a thread reply on the origin message, typically a final summary your agent posted.
    /// Slack records it as the channel's <see cref="CodeChannelProperties.SummaryMessage"/> before archiving, so the summary remains discoverable after archival.
    /// Requires the channel to have an origin link.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task Archive(string channelId, string? summaryMessageTs = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a dedicated code channel for an agent session.
    /// The calling bot user becomes the channel's agent, and is invited to the channel.
    /// </summary>
    /// <remarks>
    /// Pass <paramref name="originChannelId"/> and <paramref name="originMessageTs"/> to link the session back to the triggering message.
    /// Slack then unfurls a code channel card in the origin conversation, and invites the author of the origin message to the new code channel.
    /// When you provide an origin link and omit <paramref name="isPrivate"/>, Slack also sets the channel's privacy to match the origin channel,
    /// and allows users who are members of the origin channel to join the code channel.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.create/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelName">
    /// A friendly display name for the code channel.
    /// Optional when <paramref name="originChannelId"/> and <paramref name="originMessageTs"/> are provided; in that case the channel is named from the origin message and re-titled automatically.
    /// Required when no origin link is given.
    /// </param>
    /// <param name="originChannelId">
    /// The channel ID where the agent session was initiated from. Must be provided together with <paramref name="originMessageTs"/>.
    /// The channel must be accessible to the calling user and must not be externally shared (Slack Connect).
    /// When <paramref name="teamId"/> is omitted with an org token, the channel is created in the same workspace as this origin channel.
    /// </param>
    /// <param name="originMessageTs">
    /// The message timestamp in the origin channel that started the agent session. Must be provided together with <paramref name="originChannelId"/>.
    /// The author of this message is automatically invited to the newly created code channel.
    /// </param>
    /// <param name="sessionId">
    /// An opaque identifier for the agent session.
    /// When provided, the call is idempotent: if a channel already exists for this session ID, it is returned instead of creating a new one.
    /// </param>
    /// <param name="isPrivate">
    /// Whether the code channel is private. When omitted, the channel matches the privacy of <paramref name="originChannelId"/>, or is public if no origin is given.
    /// An explicit value is always honored, even if it differs from the origin channel's privacy: passing <c>false</c> creates a public channel from a private origin.
    /// </param>
    /// <param name="teamId">
    /// Encoded team ID to create the channel in. Required for org tokens when <paramref name="originChannelId"/> is not provided.
    /// When omitted, the workspace is derived from the origin channel.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the code channel.</returns>
    Task<string> Create(
        string? channelName = null,
        string? originChannelId = null,
        string? originMessageTs = null,
        string? sessionId = null,
        bool? isPrivate = null,
        string? teamId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a canvas attached to a code channel, with its content as markdown and its open comment threads.
    /// Each thread includes the quoted (highlighted) canvas text the comment is anchored to and its threaded replies.
    /// </summary>
    /// <remarks>
    /// A common pattern for a comment-only plan canvas: attach the canvas as a view with <see cref="SetCanvasView"/> using <see cref="AccessLevel.Comment"/>,
    /// call this method at each turn to read the unresolved comments and fold them into your agent's context,
    /// then write the revised document back with <see cref="SetCanvasContent"/>, which preserves the comment anchors on the sections you didn't change.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.getCanvas/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the agent session channel the canvas belongs to.</param>
    /// <param name="canvasId">Encoded ID of the canvas to fetch.</param>
    /// <param name="includeResolved">Whether to include resolved comment threads in the response.</param>
    /// <param name="cancellationToken"></param>
    Task<AgentCanvasResponse> GetCanvas(string channelId, string canvasId, bool includeResolved = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the views currently attached to a code channel, across all view types.
    /// Useful for rehydrating your app's view state after a restart, before setting or removing views.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.listViews/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of the code channel to list views for.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The views attached to the channel, in creation order.</returns>
    Task<IList<AgentView>> ListViews(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a view from a code channel.
    /// Removes the view's channel tab, deletes the backing content file (for HTML, diff, and Block Kit views), and clears the view from the channel's properties.
    /// Works for any view type. Identify the view by exactly one of <paramref name="viewKey"/> or <paramref name="viewId"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.removeView/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of the code channel to remove the view from.</param>
    /// <param name="viewKey">Agent-assigned key of the view to remove. Provide exactly one of <paramref name="viewKey"/> or <paramref name="viewId"/>.</param>
    /// <param name="viewId">Encoded channel tab ID of the view to remove. Provide exactly one of <paramref name="viewKey"/> or <paramref name="viewId"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The encoded channel tab ID of the removed view.</returns>
    Task<string> RemoveView(string channelId, string? viewKey = null, string? viewId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the full markdown content of a plan canvas attached to a code channel.
    /// Rather than overwrite the entire canvas, Slack diffs your content against the canvas's current sections and applies only the minimal set of section-level edits.
    /// Sections you didn't change keep their section IDs, and therefore any comment threads anchored to them persist after the update.
    /// </summary>
    /// <remarks>
    /// If the content you send is byte-for-byte current, the call is a no-op and doesn't bump the canvas version.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setCanvasContent/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the agent session channel the canvas is attached to.</param>
    /// <param name="canvasId">Encoded ID of the canvas whose content to replace.</param>
    /// <param name="content">The full new canvas content as markdown.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Number of section-level edit operations applied (replaced + inserted + deleted).
    /// Zero when the new content was identical to the current content.
    /// </returns>
    Task<int> SetCanvasContent(string channelId, string canvasId, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers the set of agent-defined slash commands for the calling agent in a code channel, replacing that agent's previously registered set.
    /// To add, change, or remove a single command, resend the complete list of commands you want the agent to have.
    /// Pass an empty list to clear the calling agent's commands entirely.
    /// </summary>
    /// <remarks>
    /// A code channel can host more than one agent, and each agent owns its own command set.
    /// A call replaces only the calling agent's commands and leaves other agents' commands untouched.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setCommands/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the code channel to register commands for.</param>
    /// <param name="commands">
    /// Full set of commands to register for the calling agent in this channel.
    /// Names must be unique within the set and must not collide with built-in Slack commands.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The number of commands now registered for the calling agent in this channel.</returns>
    Task<int> SetCommands(string channelId, IEnumerable<AgentCommand> commands, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets properties on a code channel. Only the properties you provide are changed.
    /// </summary>
    /// <remarks>
    /// Set a code channel's status with <see cref="IAgentsSessionsApi.SetStatus"/> and rename it with <see cref="IAgentsSessionsApi.Rename"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setProperties/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the code channel to update.</param>
    /// <param name="codeChannel">
    /// Code channel properties to set. Only provided fields are updated.
    /// The exception is <see cref="CodeChannelProperties.ContextBarItems"/>, where the list you send replaces the current set.
    /// </param>
    /// <param name="agentResource">Agent resource properties to set. Only provided fields are updated.</param>
    /// <param name="cancellationToken"></param>
    Task SetProperties(
        string channelId,
        CodeChannelProperties? codeChannel = null,
        AgentResource? agentResource = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates an HTML view in a code channel, rendered as a tab in the channel.
    /// The first call for a <paramref name="viewKey"/> creates the backing content and a channel tab;
    /// subsequent calls for the same key update it in-place and bump up its <see cref="AgentViewResponse.ContentVersion"/>.
    /// </summary>
    /// <remarks>
    /// To link to a view from a message, use the response's <see cref="AgentViewResponse.ViewId"/> as the <c>tab_id</c> of a <see cref="RichTextChannel"/> element in a <see cref="RichTextBlock"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setView/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the code channel to render the view in.</param>
    /// <param name="viewKey">
    /// Agent-assigned stable identity for the view (e.g. the source file path on the agent's machine).
    /// Calls with the same view key update the same view.
    /// </param>
    /// <param name="content">
    /// A full, self-contained HTML document.
    /// It is rendered in a sandboxed context with a restricted Content-Security-Policy; use <paramref name="cspResourceDomains"/> to declare additional origins your view loads resources from.
    /// </param>
    /// <param name="viewName">Display label for the view tab. Defaults to the last path segment of <paramref name="viewKey"/>, stripped of any <c>.html</c> or <c>.htm</c> extension.</param>
    /// <param name="cspResourceDomains">
    /// Origins the view may load scripts, styles, fonts, images, and media from, merged into the curated CDN allowlist at render time.
    /// Domains must be <c>https://</c> origins, and may not be private, reserved, or internal hosts.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AgentViewResponse> SetHtmlView(
        string channelId,
        string viewKey,
        string content,
        string? viewName = null,
        IEnumerable<string>? cspResourceDomains = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates the diff view in a code channel, which is shown in the channel's code tab.
    /// A channel has at most one diff view, so each call replaces its content and bumps up its <see cref="AgentViewResponse.ContentVersion"/>.
    /// </summary>
    /// <remarks>
    /// To link to a view from a message, use the response's <see cref="AgentViewResponse.ViewId"/> as the <c>tab_id</c> of a <see cref="RichTextChannel"/> element in a <see cref="RichTextBlock"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setView/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the code channel to render the view in.</param>
    /// <param name="content">
    /// Raw unified diff text, in standard unified diff format (i.e. the output of <c>git diff</c>).
    /// The viewer parses per-file changes from <c>diff --git</c> headers; commit metadata is not extracted or displayed.
    /// </param>
    /// <param name="baseBranch">Base branch name for display purposes.</param>
    /// <param name="headBranch">Head branch name for display purposes.</param>
    /// <param name="cancellationToken"></param>
    Task<AgentViewResponse> SetDiffView(
        string channelId,
        string content,
        string? baseBranch = null,
        string? headBranch = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a Block Kit view in a code channel, rendered as a tab in the channel.
    /// The first call for a <paramref name="viewKey"/> creates the backing content and a channel tab;
    /// subsequent calls for the same key update it in-place and bump up its <see cref="AgentViewResponse.ContentVersion"/>.
    /// </summary>
    /// <remarks>
    /// Interactive elements in a Block Kit view work the same way they do in messages.
    /// When a user interacts with one, your app receives a <see cref="Interaction.BlockActionRequest"/>, and external select menus send your app a <see cref="Interaction.BlockOptionsRequest"/>.
    /// Only the agent bot that owns the view receives its interactions.
    /// To link to a view from a message, use the response's <see cref="AgentViewResponse.ViewId"/> as the <c>tab_id</c> of a <see cref="RichTextChannel"/> element in a <see cref="RichTextBlock"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setView/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the code channel to render the view in.</param>
    /// <param name="viewKey">Agent-assigned stable identity for the view. Calls with the same view key update the same view.</param>
    /// <param name="blocks">Block Kit blocks to render in the view tab.</param>
    /// <param name="viewName">Display label for the view tab. Defaults to the last path segment of <paramref name="viewKey"/>.</param>
    /// <param name="cancellationToken"></param>
    Task<AgentViewResponse> SetBlockKitView(
        string channelId,
        string viewKey,
        IEnumerable<Block> blocks,
        string? viewName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches a canvas to a code channel as a view, rendered as a tab in the channel, or updates an existing canvas view.
    /// The first call for a <paramref name="viewKey"/> creates a channel tab; subsequent calls for the same key update it in-place
    /// and bump up its <see cref="AgentViewResponse.ContentVersion"/>.
    /// </summary>
    /// <remarks>
    /// To link to a view from a message, use the response's <see cref="AgentViewResponse.ViewId"/> as the <c>tab_id</c> of a <see cref="RichTextChannel"/> element in a <see cref="RichTextBlock"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.setView/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="channelId">ID of the code channel to render the view in.</param>
    /// <param name="viewKey">Agent-assigned stable identity for the view. Calls with the same view key update the same view.</param>
    /// <param name="canvasId">Encoded ID of the canvas to attach as the view.</param>
    /// <param name="accessLevel">
    /// Access level granted to the channel for the canvas tab: <see cref="AccessLevel.Read"/>, <see cref="AccessLevel.Write"/> (Slack's default), or <see cref="AccessLevel.Comment"/>.
    /// Use <see cref="AccessLevel.Comment"/> to grant channel members comment access (read and comment, no editing) so the agent remains the sole author of the canvas text.
    /// </param>
    /// <param name="agentContentHash">
    /// Hash of the canvas-derived markdown the agent last wrote, recorded so the agent can later detect human edits to the canvas. Opaque to Slack.
    /// </param>
    /// <param name="viewName">Display label for the view tab. Defaults to the last path segment of <paramref name="viewKey"/>.</param>
    /// <param name="cancellationToken"></param>
    Task<AgentViewResponse> SetCanvasView(
        string channelId,
        string viewKey,
        string canvasId,
        AccessLevel? accessLevel = null,
        string? agentContentHash = null,
        string? viewName = null,
        CancellationToken cancellationToken = default);
}

public class AgentsConversationsApi(ISlackApiClient client) : IAgentsConversationsApi
{
    public Task Archive(string channelId, string? summaryMessageTs = null, CancellationToken cancellationToken = default) =>
        client.Post("agents.conversations.archive", new Args
            {
                { "channel_id", channelId },
                { "summary_message_ts", summaryMessageTs }
            }, cancellationToken);

    public async Task<string> Create(
        string? channelName = null,
        string? originChannelId = null,
        string? originMessageTs = null,
        string? sessionId = null,
        bool? isPrivate = null,
        string? teamId = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<ChannelIdResponse>("agents.conversations.create", new Args
            {
                { "name", channelName },
                { "origin_channel_id", originChannelId },
                { "origin_message_ts", originMessageTs },
                { "session_id", sessionId },
                { "is_private", isPrivate },
                { "team_id", teamId }
            }, cancellationToken).ConfigureAwait(false))
        .ChannelId;

    public Task<AgentCanvasResponse> GetCanvas(string channelId, string canvasId, bool includeResolved = false, CancellationToken cancellationToken = default) =>
        client.Post<AgentCanvasResponse>("agents.conversations.getCanvas", new Args
            {
                { "channel", channelId },
                { "canvas_id", canvasId },
                { "include_resolved", includeResolved }
            }, cancellationToken);

    public async Task<IList<AgentView>> ListViews(string channelId, CancellationToken cancellationToken = default) =>
        (await client.Post<AgentViewListResponse>("agents.conversations.listViews", new Args { { "channel_id", channelId } }, cancellationToken).ConfigureAwait(false))
        .Views;

    public async Task<string> RemoveView(string channelId, string? viewKey = null, string? viewId = null, CancellationToken cancellationToken = default) =>
        (await client.Post<AgentViewRemovalResponse>("agents.conversations.removeView", new Args
            {
                { "channel_id", channelId },
                { "view_key", viewKey },
                { "view_id", viewId }
            }, cancellationToken).ConfigureAwait(false))
        .ViewId;

    public async Task<int> SetCanvasContent(string channelId, string canvasId, string content, CancellationToken cancellationToken = default) =>
        (await client.Post<CanvasContentUpdateResponse>("agents.conversations.setCanvasContent", new Args
            {
                { "channel", channelId },
                { "canvas_id", canvasId },
                { "content", content }
            }, cancellationToken).ConfigureAwait(false))
        .SectionsChangedCount;

    public async Task<int> SetCommands(string channelId, IEnumerable<AgentCommand> commands, CancellationToken cancellationToken = default) =>
        (await client.Post<AgentCommandsResponse>("agents.conversations.setCommands", new Args
            {
                { "channel_id", channelId },
                { "commands", commands }
            }, cancellationToken).ConfigureAwait(false))
        .CommandCount;

    public Task SetProperties(
        string channelId,
        CodeChannelProperties? codeChannel = null,
        AgentResource? agentResource = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("agents.conversations.setProperties", new Args
            {
                { "channel_id", channelId },
                { "code_channel", codeChannel },
                { "agent_resource", agentResource }
            }, cancellationToken);

    public Task<AgentViewResponse> SetHtmlView(
        string channelId,
        string viewKey,
        string content,
        string? viewName = null,
        IEnumerable<string>? cspResourceDomains = null,
        CancellationToken cancellationToken = default
    ) =>
        SetView(new Args
            {
                { "channel_id", channelId },
                { "type", AgentViewType.Html },
                { "view_key", viewKey },
                { "content", content },
                { "name", viewName },
                { "csp", cspResourceDomains is null ? null : new ContentSecurityPolicy(cspResourceDomains.ToList()) }
            }, cancellationToken);

    public Task<AgentViewResponse> SetDiffView(
        string channelId,
        string content,
        string? baseBranch = null,
        string? headBranch = null,
        CancellationToken cancellationToken = default
    ) =>
        SetView(new Args
            {
                { "channel_id", channelId },
                { "type", AgentViewType.Diff },
                { "content", content },
                { "base_branch", baseBranch },
                { "head_branch", headBranch }
            }, cancellationToken);

    public Task<AgentViewResponse> SetBlockKitView(
        string channelId,
        string viewKey,
        IEnumerable<Block> blocks,
        string? viewName = null,
        CancellationToken cancellationToken = default
    ) =>
        SetView(new Args
            {
                { "channel_id", channelId },
                { "type", AgentViewType.BlockKit },
                { "view_key", viewKey },
                { "blocks", blocks },
                { "name", viewName }
            }, cancellationToken);

    public Task<AgentViewResponse> SetCanvasView(
        string channelId,
        string viewKey,
        string canvasId,
        AccessLevel? accessLevel = null,
        string? agentContentHash = null,
        string? viewName = null,
        CancellationToken cancellationToken = default
    ) =>
        SetView(new Args
            {
                { "channel_id", channelId },
                { "type", AgentViewType.Canvas },
                { "view_key", viewKey },
                { "canvas_id", canvasId },
                { "access_level", accessLevel },
                { "agent_content_hash", agentContentHash },
                { "name", viewName }
            }, cancellationToken);

    private Task<AgentViewResponse> SetView(Args args, CancellationToken cancellationToken) =>
        client.Post<AgentViewResponse>("agents.conversations.setView", args, cancellationToken);

    private record ContentSecurityPolicy(IList<string> ResourceDomains);
}
