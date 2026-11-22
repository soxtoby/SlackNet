using System.Collections.Generic;

namespace SlackNet.WebApi;

public class MigrationResponse
{
    public string TeamId { get; set; } = null!;
    public string EnterpriseId { get; set; } = null!;
    public IDictionary<string, string> UserIdMap { get; set; } = new Dictionary<string, string>();
    public string[]? InvalidUserIds { get; set; }
}