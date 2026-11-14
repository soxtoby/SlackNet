#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// The allow list of IP blocks and ASNs in an Enterprise organization's configuration.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.audit.anomaly.allow.getItem/">Slack documentation</a> for more information.</remarks>
public class AnomalyAllowList
{
    /// <summary>
    /// Allow list of IPv4 addresses using CIDR notation in the Enterprise organization configuration.
    /// </summary>
    public IList<string> TrustedCidr { get; set; } = [];

    /// <summary>
    /// Allow list of Autonomous System Numbers (ASN) in the Enterprise organization configuration.
    /// </summary>
    public IList<long> TrustedAsns { get; set; } = [];
}
