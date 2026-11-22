using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists, publishes and auto-enables Slackbot skills owned by an Enterprise organization or standalone workspace.
/// These methods require a user token from an app installed on the Enterprise organization or on the standalone workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// The caller must be an admin or owner of the Enterprise organization or standalone workspace, or hold the org-scoped Skill Manager role.
/// </summary>
public interface IAdminSlackbotSkillsApi
{
    /// <summary>
    /// Lists metadata for skills owned by the organization, including invite-only skills.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The results include invite-only skills, but only as metadata.
    /// <see cref="Skill.Instructions"/>, <see cref="Skill.ExamplePrompt"/>, <see cref="Skill.McpServers"/>, <see cref="Skill.Access"/> and <see cref="Skill.Permalink"/> are never returned,
    /// and appearing in this list doesn't grant access to the skill canvas.
    /// Built-in skills and skill sets aren't included.
    /// </para>
    /// <para>Publication and deployment are independent: an unpublished skill can still be auto-enabled.</para>
    /// <para>
    /// A filtered page may contain fewer results than <paramref name="limit"/>, including zero.
    /// Always follow <see cref="ResponseMetadata.NextCursor"/> until it is empty, keeping filters and sort settings unchanged.
    /// Results changing during traversal may move between pages; this is not a snapshot.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.slackbot.skills.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="category">Only return skills in this category.</param>
    /// <param name="creatorIds">Only return skills created by these users. Accepts U or W user IDs.</param>
    /// <param name="publishStatus">Filter by publication to the org catalog. Defaults to <see cref="PublishStatus.All"/>.</param>
    /// <param name="autoEnabledStatus">Filter by org deployment, independently of publication. Defaults to <see cref="SkillAutoEnabledStatus.All"/>.</param>
    /// <param name="sort">Sort across the inventory. Defaults to <see cref="SkillSort.Name"/>.</param>
    /// <param name="sortDirection">Sort direction; file ID breaks ties. Defaults to <see cref="SortDirection.Ascending"/>.</param>
    /// <param name="limit">The maximum number of skills to return. Fewer may be returned even when more remain.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="SkillListResponse.ResponseMetadata"/>.
    /// Keep all filters and sorting unchanged. Pages can be empty while a continuation remains; continue until <see cref="ResponseMetadata.NextCursor"/> is empty.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<SkillListResponse> List(
        string? category = null,
        IEnumerable<string>? creatorIds = null,
        PublishStatus? publishStatus = null,
        SkillAutoEnabledStatus? autoEnabledStatus = null,
        SkillSort? sort = null,
        SortDirection? sortDirection = null,
        int limit = 100,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes Slackbot skills to an organization's catalog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Publishing an invite-only skill shares its canvas, including its instructions, with the whole organization.
    /// If a skill canvas isn't already shared with the whole organization, publishing grants org-wide read access.
    /// Existing org-wide access levels are kept.
    /// This requires the caller to have permission to edit and share that canvas; otherwise the skill fails with <c>restricted_action</c>.
    /// </para>
    /// <para>Publishing doesn't auto-enable a skill; use <see cref="SetAutoEnabled"/> for that.</para>
    /// <para>
    /// Duplicate IDs are processed once, in the order they first appear.
    /// Skills that are already published succeed, and each skill is processed independently.
    /// If any fail, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_skills</c> error,
    /// and the skills that succeeded stay published.
    /// Read the skills that failed with <see cref="SlackException.ErrorField{T}"/> using <c>failed_skills</c>, as a list of <see cref="SkillFailure"/>.
    /// If sharing succeeds but publication fails, the org-wide access remains and you can retry the request.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.slackbot.skills.publish/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillIds">Skill IDs. Successful items are not rolled back if other items fail.</param>
    /// <param name="cancellationToken"></param>
    Task Publish(IEnumerable<string> skillIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets who a Slackbot skill is automatically turned on for.
    /// </summary>
    /// <remarks>
    /// <para>Auto-enable is independent of publishing to the catalog.</para>
    /// <para>
    /// This method doesn't grant access to the skill.
    /// If some groups can't view the skill, a <see cref="SlackException"/> is thrown with a <c>skill_not_visible_to_audience</c> error.
    /// Read the groups that can't view it with <see cref="SlackException.ErrorField{T}"/> using <c>usergroup_ids</c>, as a list of strings.
    /// </para>
    /// <para>
    /// Group membership and canvas access can change after the call, and Slackbot still checks access each time the skill runs.
    /// If the call fails with <c>internal_error</c>, retrying the same request is safe.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.slackbot.skills.setAutoEnabled/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillId">Canvas skill to deploy.</param>
    /// <param name="permissionType">Deployment audience.</param>
    /// <param name="usergroupIds">
    /// User groups for <see cref="SkillAutoEnabledPermissionType.NamedEntities"/>. Duplicates are stored once.
    /// Omit for other permission types.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// The deployment state after the change.
    /// The returned <see cref="SkillAutoEnabled.UsergroupIds"/> are sorted,
    /// and are empty for <see cref="SkillAutoEnabledPermissionType.Everyone"/> and <see cref="SkillAutoEnabledPermissionType.NoOne"/>.
    /// </returns>
    Task<SkillAutoEnabled> SetAutoEnabled(
        string skillId,
        SkillAutoEnabledPermissionType permissionType,
        IEnumerable<string>? usergroupIds = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes Slackbot skills from the org catalog, preserving access, libraries, and deployments.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unpublishing doesn't delete the skill or change its canvas access, people's libraries, or auto-enable settings.
    /// Skills that are already unpublished succeed, and the canvas doesn't need to be shared org-wide to be unpublished.
    /// </para>
    /// <para>
    /// Duplicate IDs and partial failures work the same way as in <see cref="Publish"/>.
    /// If any skills fail, a <see cref="SlackException"/> is thrown with a <c>failed_for_some_skills</c> error,
    /// and the skills that succeeded stay unpublished.
    /// Read the skills that failed with <see cref="SlackException.ErrorField{T}"/> using <c>failed_skills</c>, as a list of <see cref="SkillFailure"/>.
    /// </para>
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.slackbot.skills.unpublish/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillIds">Skill IDs. Successful items are not rolled back if other items fail.</param>
    /// <param name="cancellationToken"></param>
    Task Unpublish(IEnumerable<string> skillIds, CancellationToken cancellationToken = default);
}

public class AdminSlackbotSkillsApi(ISlackApiClient client) : IAdminSlackbotSkillsApi
{
    public Task<SkillListResponse> List(
        string? category = null,
        IEnumerable<string>? creatorIds = null,
        PublishStatus? publishStatus = null,
        SkillAutoEnabledStatus? autoEnabledStatus = null,
        SkillSort? sort = null,
        SortDirection? sortDirection = null,
        int limit = 100,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<SkillListResponse>("admin.slackbot.skills.list", new Args
            {
                { "category", category },
                { "creator_ids", creatorIds },
                { "publish_status", publishStatus },
                { "auto_enabled_status", autoEnabledStatus },
                { "sort", sort },
                { "sort_dir", sortDirection },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);

    public Task Publish(IEnumerable<string> skillIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.slackbot.skills.publish", new Args { { "skill_ids", skillIds } }, cancellationToken);

    public Task<SkillAutoEnabled> SetAutoEnabled(
        string skillId,
        SkillAutoEnabledPermissionType permissionType,
        IEnumerable<string>? usergroupIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<SkillAutoEnabled>("admin.slackbot.skills.setAutoEnabled", new Args
            {
                { "skill_id", skillId },
                { "permission_type", permissionType },
                { "usergroup_ids", usergroupIds }
            }, cancellationToken);

    public Task Unpublish(IEnumerable<string> skillIds, CancellationToken cancellationToken = default) =>
        client.Post("admin.slackbot.skills.unpublish", new Args { { "skill_ids", skillIds } }, cancellationToken);
}
