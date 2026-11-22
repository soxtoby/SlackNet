using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Reads and writes the allow list of IP blocks and ASNs in an Enterprise organization's configuration.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAuditAnomalyAllowApi
{
    /// <summary>
    /// Allows Enterprise organization admins to read the allow list of IP blocks and ASNs from the enterprise configuration.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.audit.anomaly.allow.getItem/">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<AnomalyAllowList> GetItem(CancellationToken cancellationToken = default);

    /// <summary>
    /// Allows Enterprise organization admins to write/overwrite the allow list of IP blocks and ASNs from the enterprise configuration.
    /// Writing to this endpoint will overwrite previously added IP ranges and ASNs.
    /// If you are adding new IP ranges and ASNs, you must include all of the previously added IP ranges and ASNs, or they will be overwritten.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.audit.anomaly.allow.updateItem/">Slack documentation</a> for more information.</remarks>
    /// <param name="trustedCidr">Allow list of IPv4 addresses using CIDR notation in the Enterprise organization configuration.</param>
    /// <param name="trustedAsns">Allow list of Autonomous System Numbers (ASN) in the Enterprise organization configuration.</param>
    /// <param name="cancellationToken"></param>
    Task UpdateItem(
        IEnumerable<string>? trustedCidr = null,
        IEnumerable<long>? trustedAsns = null,
        CancellationToken cancellationToken = default);
}

public class AdminAuditAnomalyAllowApi(ISlackApiClient client) : IAdminAuditAnomalyAllowApi
{
    public Task<AnomalyAllowList> GetItem(CancellationToken cancellationToken = default) =>
        client.Post<AnomalyAllowList>("admin.audit.anomaly.allow.getItem", new Args(), cancellationToken);

    public Task UpdateItem(
        IEnumerable<string>? trustedCidr = null,
        IEnumerable<long>? trustedAsns = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post("admin.audit.anomaly.allow.updateItem", new Args
            {
                { "trusted_cidr", trustedCidr },
                { "trusted_asns", trustedAsns }
            }, cancellationToken);
}
