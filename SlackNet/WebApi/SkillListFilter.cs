namespace SlackNet.WebApi;

/// <summary>
/// Which skills to list with <see cref="ISlackbotSkillsApi.List"/>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackbot.skills.list/">Slack documentation</a> for more information.</remarks>
public enum SkillListFilter
{
    /// <summary>
    /// Skills enabled for the caller, including auto-enabled skills and skills from skill sets.
    /// </summary>
    Library,

    /// <summary>
    /// Skills the caller created.
    /// </summary>
    Owned,

    /// <summary>
    /// The organization's official skills.
    /// </summary>
    Catalog
}
