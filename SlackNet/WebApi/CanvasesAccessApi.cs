#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface ICanvasesAccessApi
{
    /// <summary>
    /// Remove access to a canvas for specified entities.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/canvases.acccess.delete">Slack documentation</a> for more information.</remarks>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="channelIds">List of channels you wish to update access for.</param>
    /// <param name="userIds">.List of users you wish to update access for</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets the access level to a canvas for specified entities.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/canvases.acccess.set">Slack documentation</a> for more information.</remarks>
    /// <param name="accessLevel">Desired level of access.</param>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="channelIds">List of channels you wish to update access for. Can only be used if <see cref="userIds"/> is not provided.</param>
    /// <param name="userIds">List of users you wish to update access for. Can only be used if <see cref="channelIds"/> is not provided.</param>
    /// <param name="cancellationToken"></param>
    Task Set(AccessLevel accessLevel, string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default);
}

public class CanvasesAccessApi(ISlackApiClient client) : ICanvasesAccessApi
{
    public Task Delete(string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default) =>
        client.Post("canvases.access.delete", new Args
            {
                { "canvas_id", canvasId },
                { "channel_ids", channelIds },
                { "user_ids", userIds }
            }, cancellationToken);

    public Task Set(AccessLevel accessLevel, string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default) =>
        client.Post("canvases.access.set", new Args
            {
                { "access_level", accessLevel },
                { "canvas_id", canvasId },
                { "channel_ids", channelIds },
                { "user_ids", userIds }
            }, cancellationToken);
}