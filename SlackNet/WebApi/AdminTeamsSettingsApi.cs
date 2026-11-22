using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Gets and sets the settings of workspaces in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminTeamsSettingsApi
{
    /// <summary>
    /// Fetch information about settings in a workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.info/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">The workspace to fetch the settings of.</param>
    /// <param name="cancellationToken"></param>
    Task<Team> Info(string teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the default channels of a workspace.
    /// </summary>
    /// <remarks>
    /// Default channels are joined by new members of a workspace automatically.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.setDefaultChannels/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">ID for the workspace to set the default channel for.</param>
    /// <param name="channelIds">An array of channel IDs.</param>
    /// <param name="cancellationToken"></param>
    Task SetDefaultChannels(string teamId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the description of a given workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.setDescription/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">ID for the workspace to set the description for.</param>
    /// <param name="description">The new description for the workspace.</param>
    /// <param name="cancellationToken"></param>
    Task SetDescription(string teamId, string description, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the discoverability of a given workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.setDiscoverability/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">The ID of the workspace to set discoverability on.</param>
    /// <param name="discoverability">This workspace's discovery setting.</param>
    /// <param name="cancellationToken"></param>
    Task SetDiscoverability(string teamId, TeamDiscoverability discoverability, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the icon of a workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.setIcon/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">ID for the workspace to set the icon for.</param>
    /// <param name="imageUrl">Image URL for the icon.</param>
    /// <param name="cancellationToken"></param>
    Task SetIcon(string teamId, string imageUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the name of a given workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.setName/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">ID for the workspace to set the name for.</param>
    /// <param name="teamName">The new name of the workspace.</param>
    /// <param name="cancellationToken"></param>
    Task SetName(string teamId, string teamName, CancellationToken cancellationToken = default);
}

public class AdminTeamsSettingsApi(ISlackApiClient client) : IAdminTeamsSettingsApi
{
    public async Task<Team> Info(string teamId, CancellationToken cancellationToken = default) =>
        (await client.Post<TeamResponse>("admin.teams.settings.info", new Args { { "team_id", teamId } }, cancellationToken).ConfigureAwait(false))
        .Team;

    public Task SetDefaultChannels(string teamId, IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        client.Get("admin.teams.settings.setDefaultChannels", new Args
            {
                { "team_id", teamId },
                { "channel_ids", channelIds }
            }, cancellationToken);

    public Task SetDescription(string teamId, string description, CancellationToken cancellationToken = default) =>
        client.Post("admin.teams.settings.setDescription", new Args
            {
                { "team_id", teamId },
                { "description", description }
            }, cancellationToken);

    public Task SetDiscoverability(string teamId, TeamDiscoverability discoverability, CancellationToken cancellationToken = default) =>
        client.Post("admin.teams.settings.setDiscoverability", new Args
            {
                { "team_id", teamId },
                { "discoverability", discoverability }
            }, cancellationToken);

    public Task SetIcon(string teamId, string imageUrl, CancellationToken cancellationToken = default) =>
        client.Get("admin.teams.settings.setIcon", new Args
            {
                { "team_id", teamId },
                { "image_url", imageUrl }
            }, cancellationToken);

    public Task SetName(string teamId, string teamName, CancellationToken cancellationToken = default) =>
        client.Post("admin.teams.settings.setName", new Args
            {
                { "team_id", teamId },
                { "name", teamName }
            }, cancellationToken);
}
