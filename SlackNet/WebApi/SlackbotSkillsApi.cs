using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface ISlackbotSkillsApi
{
    /// <summary>
    /// Creates a skill from markdown instructions, or turns an existing canvas the caller owns into a skill.
    /// One, and only one, of <paramref name="instructions"/> and <paramref name="canvasId"/> is required.
    /// </summary>
    /// <remarks>
    /// The created skill is added to the caller's own Slackbot library when possible, and isn't turned on for anyone else.
    /// To share the skill, share its canvas.
    /// This method is not an upsert. Save the returned <see cref="Skill.Id"/> and pass it to <see cref="Update"/> for later changes,
    /// for example when deploying a skill from CI.
    /// See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.create/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="displayName">Human-readable label. Not unique.</param>
    /// <param name="instructions">Markdown body of the skill. Send exactly one of <paramref name="instructions"/> or <paramref name="canvasId"/>.</param>
    /// <param name="canvasId">Canvas the caller owns to turn into a skill. Send exactly one of <paramref name="instructions"/> or <paramref name="canvasId"/>.</param>
    /// <param name="description">When Slackbot should use the skill.</param>
    /// <param name="category">
    /// One of the skill categories: <c>communication</c>, <c>customer_support</c>, <c>data_and_analytics</c>, <c>engineering</c>,
    /// <c>product_management</c>, <c>productivity</c>, <c>professional_development</c>, <c>project_management</c>, <c>sales</c>,
    /// <c>system</c>, <c>team_management</c> or <c>other</c>.
    /// </param>
    /// <param name="examplePrompt">May contain <c>{skill}</c>, which becomes the skill mention.</param>
    /// <param name="mcpServers">Apps the skill needs. Declaring an app doesn't grant access to it.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created skill.</returns>
    Task<Skill> Create(
        string displayName,
        string? instructions = null,
        string? canvasId = null,
        string? description = null,
        string? category = null,
        string? examplePrompt = null,
        IEnumerable<McpServer>? mcpServers = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a canvas skill and its underlying canvas.
    /// The skill is also removed from every library, skill set, and the organization catalog.
    /// </summary>
    /// <remarks>
    /// To turn a skill off for yourself without deleting it, use <see cref="ISlackbotSkillsLibraryApi.Remove"/> instead.
    /// Built-in skills can't be deleted.
    /// See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.delete/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillId">Skill to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string skillId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns one Slackbot skill, including its instructions.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.info/">Slack documentation</a> for more information.</remarks>
    /// <param name="skillId">Skill to fetch: a canvas file ID or a <c>builtin:</c> reference.</param>
    /// <param name="cancellationToken"></param>
    Task<Skill> Info(string skillId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists Slackbot skills from the caller's point of view.
    /// Instructions aren't included; use <see cref="Info"/> to fetch them.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="filter">Which skills to list. Defaults to <see cref="SkillListFilter.Library"/>.</param>
    /// <param name="category">Only return skills in this category.</param>
    /// <param name="limit">The maximum number of skills to return. Fewer may be returned even when more remain.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="SkillListResponse.ResponseMetadata"/>.
    /// Must be used with the same <paramref name="filter"/> and <paramref name="category"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<SkillListResponse> List(
        SkillListFilter? filter = null,
        string? category = null,
        int limit = 100,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a Slackbot skill's instructions or metadata.
    /// Only the arguments you send are changed.
    /// </summary>
    /// <remarks>
    /// Sending <paramref name="instructions"/> replaces the whole document, and sending <paramref name="mcpServers"/> replaces the whole list.
    /// Unchanged values are skipped where they can be compared, but markdown normalization can still cause the instructions to be rewritten.
    /// An update can partially apply if it fails partway through, and retrying the same request is safe.
    /// See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.update/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillId">Skill to change.</param>
    /// <param name="displayName">Human-readable label.</param>
    /// <param name="instructions">Markdown that replaces the whole document.</param>
    /// <param name="description">When Slackbot should use the skill. Send an empty string to clear.</param>
    /// <param name="category">One of the skill categories, as for <see cref="Create"/>. Send an empty string to clear.</param>
    /// <param name="examplePrompt">May contain <c>{skill}</c>. Send an empty string to clear.</param>
    /// <param name="mcpServers">
    /// Apps the skill needs. Replaces the whole list; send an empty list to clear.
    /// A skill's <see cref="Skill.McpServers"/> can be passed back, and only their <see cref="McpServer"/> properties are sent.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>The updated skill.</returns>
    Task<Skill> Update(
        string skillId,
        string? displayName = null,
        string? instructions = null,
        string? description = null,
        string? category = null,
        string? examplePrompt = null,
        IEnumerable<McpServer>? mcpServers = null,
        CancellationToken cancellationToken = default);
}

public class SlackbotSkillsApi(ISlackApiClient client) : ISlackbotSkillsApi
{
    public async Task<Skill> Create(
        string displayName,
        string? instructions = null,
        string? canvasId = null,
        string? description = null,
        string? category = null,
        string? examplePrompt = null,
        IEnumerable<McpServer>? mcpServers = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<SkillResponse>("slackbot.skills.create", new Args
            {
                { "display_name", displayName },
                { "instructions", instructions },
                { "canvas_id", canvasId },
                { "description", description },
                { "category", category },
                { "example_prompt", examplePrompt },
                { "mcp_servers", ServerReferences(mcpServers) }
            }, cancellationToken).ConfigureAwait(false))
        .Skill;

    public Task Delete(string skillId, CancellationToken cancellationToken = default) =>
        client.Post("slackbot.skills.delete", new Args { { "skill_id", skillId } }, cancellationToken);

    public async Task<Skill> Info(string skillId, CancellationToken cancellationToken = default) =>
        (await client.Post<SkillResponse>("slackbot.skills.info", new Args { { "skill_id", skillId } }, cancellationToken).ConfigureAwait(false))
        .Skill;

    public Task<SkillListResponse> List(
        SkillListFilter? filter = null,
        string? category = null,
        int limit = 100,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<SkillListResponse>("slackbot.skills.list", new Args
            {
                { "filter", filter },
                { "category", category },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);

    public async Task<Skill> Update(
        string skillId,
        string? displayName = null,
        string? instructions = null,
        string? description = null,
        string? category = null,
        string? examplePrompt = null,
        IEnumerable<McpServer>? mcpServers = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<SkillResponse>("slackbot.skills.update", new Args
            {
                { "skill_id", skillId },
                { "display_name", displayName },
                { "instructions", instructions },
                { "description", description },
                { "category", category },
                { "example_prompt", examplePrompt },
                { "mcp_servers", ServerReferences(mcpServers) }
            }, cancellationToken).ConfigureAwait(false))
        .Skill;

    private static IList<McpServer>? ServerReferences(IEnumerable<McpServer>? mcpServers) =>
        mcpServers?.Select(s => s.AsReference()).ToList();
}
