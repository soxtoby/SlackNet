using System.Collections.Generic;

namespace SlackNet.WebApi;

public class MigrationResponse
{
    public string TeamId { get; set; }
    public string EnterpriseId { get; set; }
    public IDictionary<string, string> UserIdMap { get; set; } = new Dictionary<string, string>();
    public string[] InvalidUserIds { get; set; }
}