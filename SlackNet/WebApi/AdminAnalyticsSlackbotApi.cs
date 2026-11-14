#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Retrieves analytics for Slackbot usage in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAnalyticsSlackbotApi
{
    /// <summary>
    /// Retrieves per-user Slackbot skill usage for an Enterprise organization on a single date.
    /// Each row lists the skills a user used, added, or shared, along with how many times the user used, added, and shared each one.
    /// </summary>
    /// <remarks>
    /// <paramref name="limit"/> counts skill entries across all rows, not rows.
    /// When a page ends partway through a user's skills, the rest continue in a row with the same <see cref="SlackbotSkillUsage.UserId"/>
    /// at the start of the next page, so merge rows that share a <see cref="SlackbotSkillUsage.UserId"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.slackbot.skills/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="date">
    /// The date to retrieve the analytics data for, in UTC. If not provided, defaults to the latest date with available data.
    /// The response's <see cref="SlackbotSkillUsageResponse.AvailableDateRange"/> gives the earliest and latest dates you can request.
    /// </param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="SlackbotSkillUsageResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">Maximum number of skill entries to return per page, counted across all rows. Defaults to 500.</param>
    /// <param name="cancellationToken"></param>
    Task<SlackbotSkillUsageResponse> Skills(
        DateTime? date = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves per-user Slackbot tool usage for an Enterprise organization on a single date.
    /// Each row lists the user-facing tools Slackbot used on a user's behalf, along with how many times Slackbot used each one.
    /// </summary>
    /// <remarks>
    /// <paramref name="limit"/> counts tool entries across all rows, not rows.
    /// When a page ends partway through a user's tools, the rest continue in a row with the same <see cref="SlackbotToolUsage.UserId"/>
    /// at the start of the next page, so merge rows that share a <see cref="SlackbotToolUsage.UserId"/>.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.slackbot.tools/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="date">
    /// The date to retrieve the analytics data for, in UTC. If not provided, defaults to the latest date with available data.
    /// The response's <see cref="SlackbotToolUsageResponse.AvailableDateRange"/> gives the earliest and latest dates you can request.
    /// </param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="SlackbotToolUsageResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">Maximum number of tool entries to return per page, counted across all rows. Defaults to 500.</param>
    /// <param name="cancellationToken"></param>
    Task<SlackbotToolUsageResponse> Tools(
        DateTime? date = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class AdminAnalyticsSlackbotApi(ISlackApiClient client) : IAdminAnalyticsSlackbotApi
{
    public Task<SlackbotSkillUsageResponse> Skills(
        DateTime? date = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<SlackbotSkillUsageResponse>("admin.analytics.slackbot.skills", new Args
            {
                { "date", date },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public Task<SlackbotToolUsageResponse> Tools(
        DateTime? date = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<SlackbotToolUsageResponse>("admin.analytics.slackbot.tools", new Args
            {
                { "date", date },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
