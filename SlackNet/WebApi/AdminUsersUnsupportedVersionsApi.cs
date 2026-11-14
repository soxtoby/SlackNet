#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Exports the members of an Enterprise organization who are using unsupported software.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminUsersUnsupportedVersionsApi
{
    /// <summary>
    /// Ask Slackbot to send you an export listing all workspace members using unsupported software, presented as a zipped CSV file.
    /// </summary>
    /// <remarks>
    /// Due to the potential time to generate this data, the zip file is exported offline and delivered using Slack.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.users.unsupportedVersions.export/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="dateSessionsStarted">A date to start looking for user sessions. If not provided will start six months ago.</param>
    /// <param name="dateEndOfSupport">
    /// The date of past or upcoming end of support cycles. If not provided will include all announced end of support cycles.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task Export(DateTime? dateSessionsStarted = null, DateTime? dateEndOfSupport = null, CancellationToken cancellationToken = default);
}

public class AdminUsersUnsupportedVersionsApi(ISlackApiClient client) : IAdminUsersUnsupportedVersionsApi
{
    public Task Export(DateTime? dateSessionsStarted = null, DateTime? dateEndOfSupport = null, CancellationToken cancellationToken = default) =>
        client.Get("admin.users.unsupportedVersions.export", new Args
            {
                { "date_sessions_started", dateSessionsStarted?.ToTimestampNumber() },
                { "date_end_of_support", dateEndOfSupport?.ToTimestampNumber() }
            }, cancellationToken);
}
