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
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/canvases.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="title">Title of the newly created canvas.</param>
    /// <param name="documentContent">Structure describing the type and value of the content to create.</param>
    /// <param name="channelId">Channel ID of the channel the canvas will be tabbed in. This is a required field for free teams.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the newly created canvas.</returns>
    Task<string> Create(string? title = null, DocumentContent? documentContent = null, string? channelId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a canvas.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/canvases.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string canvasId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove access to a canvas for specified entities.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/canvases.access.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="channelIds">List of channels you wish to update access for.</param>
    /// <param name="userIds">.List of users you wish to update access for</param>
    /// <param name="cancellationToken"></param>
    Task DeleteAccess(string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update an existing canvas.
    /// </summary>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="changes">List of changes to apply on the specified canvas.</param>
    /// <param name="cancellationToken"></param>
    Task Edit(string canvasId, IEnumerable<CanvasChange> changes, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Find sections matching the provided criteria.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/canvases.sections.lookup/">Slack documentation</a> for more information.</remarks>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="criteria">Filtering criteria.</param>
    /// <param name="cancellationToken"></param>
    Task<IList<CanvasSection>> LookupSections(string canvasId, LookupCriteria criteria, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets the access level to a canvas for specified entities.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/canvases.acccess.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="accessLevel">Desired level of access.</param>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="channelIds">List of channels you wish to update access for. Can only be used if <see cref="userIds"/> is not provided.</param>
    /// <param name="userIds">List of users you wish to update access for. Can only be used if <see cref="channelIds"/> is not provided.</param>
    /// <param name="cancellationToken"></param>
    Task SetAccess(AccessLevel accessLevel, string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default);
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
    
    public Task DeleteAccess(string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default) =>
        client.Post("canvases.access.delete", new Args
            {
                { "canvas_id", canvasId },
                { "channel_ids", channelIds },
                { "user_ids", userIds }
            }, cancellationToken);
    
    public Task Edit(string canvasId, IEnumerable<CanvasChange> changes, CancellationToken cancellationToken = default) =>
        client.Post("canvases.edit", new Args
            {
                { "canvas_id", canvasId },
                { "changes", changes }
            }, cancellationToken);
    
    public async Task<IList<CanvasSection>> LookupSections(string canvasId, LookupCriteria criteria, CancellationToken cancellationToken = default) =>
        (await client.Post<LookupResponse>("canvases.sections.lookup",
            new Args
                {
                    { "canvas_id", canvasId },
                    { "criteria", criteria },
                },
            cancellationToken).ConfigureAwait(false))
        .Sections;
    
    public Task SetAccess(AccessLevel accessLevel, string canvasId, IEnumerable<string>? channelIds = null, IEnumerable<string>? userIds = null, CancellationToken cancellationToken = default) =>
        client.Post("canvases.access.set", new Args
            {
                { "access_level", accessLevel },
                { "canvas_id", canvasId },
                { "channel_ids", channelIds },
                { "user_ids", userIds }
            }, cancellationToken);
}