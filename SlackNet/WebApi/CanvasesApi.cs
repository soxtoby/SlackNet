#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface ICanvasesApi
{
    /// <summary>
    /// Create canvas for a user.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/canvases.create">Slack documentation</a> for more information.</remarks>
    /// <param name="title">Title of the newly created canvas.</param>
    /// <param name="documentContent">Structure describing the type and value of the content to create.</param>
    /// <param name="channelId">Channel ID of the channel the canvas will be tabbed in. This is a required field for free teams.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the newly created canvas.</returns>
    Task<string> Create(string? title = null, DocumentContent? documentContent = null, string? channelId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a canvas.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/canvases.delete">Slack documentation</a> for more information.</remarks>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string canvasId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing canvas.
    /// </summary>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="changes">List of changes to apply on the specified canvas.</param>
    /// <param name="cancellationToken"></param>
    Task Edit(string canvasId, IEnumerable<CanvasChange> changes, CancellationToken cancellationToken = default);
}

public class CanvasesApi(ISlackApiClient client) : ICanvasesApi
{
    public async Task<string> Create(string? title = null, DocumentContent? documentContent = null, string? channelId = null, CancellationToken cancellationToken = default) =>
        (await client.Post<CanvasCreationResponse>("canvases.create", new Args
            {
                { "title", title },
                { "document_content", documentContent },
                { "channel_id", channelId }
            },
            cancellationToken).ConfigureAwait(false))
        .CanvasId;
    
    public Task Delete(string canvasId, CancellationToken cancellationToken = default) =>
        client.Post("canvases.delete", new Args { { "canvas_id", canvasId } }, cancellationToken);
    
    public Task Edit(string canvasId, IEnumerable<CanvasChange> changes, CancellationToken cancellationToken = default) =>
        client.Post("canvases.edit", new Args
            {
                { "canvas_id", canvasId },
                { "changes", changes }
            }, cancellationToken);
}

class CanvasCreationResponse
{
    public string CanvasId { get; set; } = null!;
}