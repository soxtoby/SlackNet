using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IWorkflowsFeaturedApi
{
    /// <summary>
    /// Add featured workflows to a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.featured.add">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to add featured workflow in.</param>
    /// <param name="triggerIds">Array of trigger IDs to add.</param>
    /// <param name="cancellationToken"></param>
    Task Add(string channelId, IEnumerable<string> triggerIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// List the featured workflows for specified channels.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.featured.list">Slack documentation</a> for more information.</remarks>
    /// <param name="channelIds">Array of channel IDs to list featured workflows for.</param>
    /// <param name="cancellationToken"></param>
    Task<FeaturedWorkflowsListResponse> List(IEnumerable<string> channelIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove featured workflows from a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.featured.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to remove featured workflow from.</param>
    /// <param name="triggerIds">Array of trigger IDs to remove.</param>
    /// <param name="cancellationToken"></param>
    Task Remove(string channelId, IEnumerable<string> triggerIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set featured workflows for a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/workflows.featured.set">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to set featured workflows in.</param>
    /// <param name="triggerIds">Array of trigger IDs that will replace any existing featured workflows in the channel.</param>
    /// <param name="cancellationToken"></param>
    Task Set(string channelId, IEnumerable<string> triggerIds, CancellationToken cancellationToken = default);
}

public class WorkflowsFeaturedApi(ISlackApiClient client) : IWorkflowsFeaturedApi
{
    public Task Add(string channelId, IEnumerable<string> triggerIds, CancellationToken cancellationToken = default) =>
        client.Post("workflows.featured.add", new Args
            {
                { "channel_id", channelId },
                { "trigger_ids", triggerIds }
            }, cancellationToken);

    public Task<FeaturedWorkflowsListResponse> List(IEnumerable<string> channelIds, CancellationToken cancellationToken = default) =>
        client.Post<FeaturedWorkflowsListResponse>("workflows.featured.list", new Args
            {
                { "channel_ids", channelIds }
            }, cancellationToken);

    public Task Remove(string channelId, IEnumerable<string> triggerIds, CancellationToken cancellationToken = default) =>
        client.Post("workflows.featured.remove", new Args
            {
                { "channel_id", channelId },
                { "trigger_ids", triggerIds }
            }, cancellationToken);

    public Task Set(string channelId, IEnumerable<string> triggerIds, CancellationToken cancellationToken = default) =>
        client.Post("workflows.featured.set", new Args
            {
                { "channel_id", channelId },
                { "trigger_ids", triggerIds }
            }, cancellationToken);
}
