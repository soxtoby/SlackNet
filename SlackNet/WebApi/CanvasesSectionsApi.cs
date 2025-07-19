#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface ICanvasesSectionsApi
{
    /// <summary>
    /// Find sections matching the provided criteria.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/canvases.sections.lookup">Slack documentation</a> for more information.</remarks>
    /// <param name="canvasId">ID of the canvas.</param>
    /// <param name="criteria">Filtering criteria.</param>
    /// <param name="cancellationToken"></param>
    Task<IList<CanvasSection>> Lookup(string canvasId, LookupCriteria criteria, CancellationToken cancellationToken = default);
}

public class CanvasesSectionsApi(ISlackApiClient client) : ICanvasesSectionsApi
{
    public async Task<IList<CanvasSection>> Lookup(string canvasId, LookupCriteria criteria, CancellationToken cancellationToken = default) =>
        (await client.Post<LookupResponse>("canvases.sections.lookup",
            new Args
                {
                    { "canvas_id", canvasId },
                    { "criteria", criteria },
                },
            cancellationToken).ConfigureAwait(false))
        .Sections;
}

class LookupResponse
{
    public IList<CanvasSection> Sections { get; set; } = [];
}