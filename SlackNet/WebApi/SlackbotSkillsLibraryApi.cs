using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface ISlackbotSkillsLibraryApi
{
    /// <summary>
    /// Turns a skill on for the caller, matching the personal library toggle in Slackbot.
    /// </summary>
    /// <remarks>
    /// If the skill already reaches the caller through an organization deployment or a skill set,
    /// this method re-enables it without adding a separate library entry. Adding a skill that is already on succeeds.
    /// This method changes only the caller's library. The skill itself, its sharing, publication, and other people's libraries are unaffected.
    /// To check the result, call <see cref="ISlackbotSkillsApi.List"/> with <see cref="SkillListFilter.Library"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.library.add/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillId">Skill to turn on for the caller.</param>
    /// <param name="cancellationToken"></param>
    Task Add(string skillId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Turns a skill off for the caller, the same as the personal library toggle in Slackbot.
    /// </summary>
    /// <remarks>
    /// For a skill the caller added directly, this method removes the library entry.
    /// For a skill supplied by an org deployment or a subscribed skill set, this method turns the skill off for the caller while leaving the deployment or set intact.
    /// Removing a skill that is already off succeeds.
    /// This method doesn't delete or unpublish the skill, change its access, or affect anyone else.
    /// To delete a skill, use <see cref="ISlackbotSkillsApi.Delete"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.library.remove/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="skillId">Skill to turn off for the caller.</param>
    /// <param name="cancellationToken"></param>
    Task Remove(string skillId, CancellationToken cancellationToken = default);
}

public class SlackbotSkillsLibraryApi(ISlackApiClient client) : ISlackbotSkillsLibraryApi
{
    public Task Add(string skillId, CancellationToken cancellationToken = default) =>
        client.Post("slackbot.skills.library.add", new Args { { "skill_id", skillId } }, cancellationToken);

    public Task Remove(string skillId, CancellationToken cancellationToken = default) =>
        client.Post("slackbot.skills.library.remove", new Args { { "skill_id", skillId } }, cancellationToken);
}
