namespace SlackNet;

/// <summary>
/// A workspace in an Enterprise organization.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.list/">Slack documentation</a> for more information.</remarks>
public class EnterpriseTeam
{
    /// <summary>
    /// Encoded ID of the workspace.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Name of the workspace.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Who is able to join this workspace, usually one of the <see cref="WebApi.TeamDiscoverability"/> values in snake_case.
    /// </summary>
    /// <remarks>
    /// Slack's example response includes <c>hidden</c>, which isn't one of the documented values,
    /// so this is a string rather than an enum, to avoid failing on values SlackNet doesn't know about.
    /// </remarks>
    public required string Discoverability { get; set; }

    /// <summary>
    /// The primary owner of the workspace.
    /// </summary>
    public required TeamPrimaryOwner PrimaryOwner { get; set; }

    /// <summary>
    /// The URL of the workspace.
    /// </summary>
    public required string TeamUrl { get; set; }
}

/// <summary>
/// The primary owner of an <see cref="EnterpriseTeam"/>.
/// </summary>
public class TeamPrimaryOwner
{
    /// <summary>
    /// Encoded user ID of the primary owner of the workspace.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Email of the primary owner of the workspace.
    /// </summary>
    public required string Email { get; set; }
}
