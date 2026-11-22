using System.Collections.Generic;

namespace SlackNet;

public class Team
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Domain { get; set; }
    public string? EmailDomain { get; set; }
    public Icons? Icon { get; set; }
    public string? EnterpriseId { get; set; }
    public string? EnterpriseName { get; set; }

    /// <summary>
    /// Whether the team is migrating to an Enterprise organization.
    /// </summary>
    public bool IsMigrating { get; set; }

    /// <summary>
    /// IDs of the channels that new members of the workspace join automatically.
    /// Only returned by <see cref="WebApi.IAdminTeamsSettingsApi.Info"/>.
    /// </summary>
    public IList<string> DefaultChannels { get; set; } = [];
}