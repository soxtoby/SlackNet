using System;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Gets logs for <a href="https://docs.slack.dev/workflows/">workflow apps</a> across an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAppsActivitiesApi
{
    /// <summary>
    /// Get logs for a specified team/org.
    /// Returns all workflow executions for an organization.
    /// Additionally, it can be used to ingest logs and to monitor errors across an organization.
    /// If multiple filters are used, filtering will be performed with a logical "and" operator.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.activities.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">The ID of the app to get activities from.</param>
    /// <param name="teamId">The team who owns this log.</param>
    /// <param name="minLogLevel">The minimum log level of the log events to be returned. Defaults to <see cref="AppActivityLogLevel.Info"/>.</param>
    /// <param name="logEventType">The event type of log events to be returned.</param>
    /// <param name="source">The source of log events to be returned.</param>
    /// <param name="componentType">The component type of log events to be returned.</param>
    /// <param name="componentId">The component ID of log events to be returned. Will be <c>FnXXXXXX</c> for functions, and <c>WfXXXXXX</c> for workflows.</param>
    /// <param name="traceId">The trace ID of log events to be returned.</param>
    /// <param name="minDateCreated">The earliest timestamp of the log to retrieve.</param>
    /// <param name="maxDateCreated">The latest timestamp of the log to retrieve.</param>
    /// <param name="sortDirection">The direction you want the data sorted by (always by timestamp).</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="AppActivityListResponse.ResponseMetadata"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AppActivityListResponse> List(
        string? appId = null,
        string? teamId = null,
        AppActivityLogLevel? minLogLevel = null,
        string? logEventType = null,
        AppActivitySource? source = null,
        AppActivityComponentType? componentType = null,
        string? componentId = null,
        string? traceId = null,
        DateTime? minDateCreated = null,
        DateTime? maxDateCreated = null,
        SortDirection? sortDirection = null,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);
}

public class AdminAppsActivitiesApi(ISlackApiClient client) : IAdminAppsActivitiesApi
{
    public Task<AppActivityListResponse> List(
        string? appId = null,
        string? teamId = null,
        AppActivityLogLevel? minLogLevel = null,
        string? logEventType = null,
        AppActivitySource? source = null,
        AppActivityComponentType? componentType = null,
        string? componentId = null,
        string? traceId = null,
        DateTime? minDateCreated = null,
        DateTime? maxDateCreated = null,
        SortDirection? sortDirection = null,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<AppActivityListResponse>("admin.apps.activities.list", new Args
            {
                { "app_id", appId },
                { "team_id", teamId },
                { "min_log_level", minLogLevel },
                { "log_event_type", logEventType },
                { "source", source },
                { "component_type", componentType },
                { "component_id", componentId },
                { "trace_id", traceId },
                { "min_date_created", minDateCreated?.ToEpochMicroseconds() },
                { "max_date_created", maxDateCreated?.ToEpochMicroseconds() },
                { "sort_direction", sortDirection },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);
}
