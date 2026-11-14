using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Blocks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IBlocksApi
{
    /// <summary>
    /// Validates blocks against the Block Kit schema.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/blocks.validate/">Slack documentation</a> for more information.</remarks>
    /// <param name="blocks">A list of structured blocks.</param>
    /// <param name="cancellationToken"></param>
    Task Validate(IEnumerable<Block> blocks, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a message payload against the Block Kit schema.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/blocks.validate/">Slack documentation</a> for more information.</remarks>
    /// <param name="message">A message payload to validate.</param>
    /// <param name="cancellationToken"></param>
    Task Validate(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a view payload against the Block Kit schema.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/blocks.validate/">Slack documentation</a> for more information.</remarks>
    /// <param name="view">A view payload to validate.</param>
    /// <param name="cancellationToken"></param>
    Task Validate(ViewDefinition view, CancellationToken cancellationToken = default);
}

public class BlocksApi(ISlackApiClient client, SlackJsonSettings jsonSettings) : IBlocksApi
{
    public Task Validate(IEnumerable<Block> blocks, CancellationToken cancellationToken = default) =>
        client.Post("blocks.validate", new Args { { "blocks", blocks } }, cancellationToken);

    public Task Validate(Message message, CancellationToken cancellationToken = default) =>
        client.Post("blocks.validate", new Args { { "message", MessagePayload(message) } }, cancellationToken);

    public Task Validate(ViewDefinition view, CancellationToken cancellationToken = default) =>
        client.Post("blocks.validate", new Args { { "view", view } }, cancellationToken);

    private Args MessagePayload(Message message) =>
        SlackApiClient.StripNullArgs(ChatApi.PopulateMessageArgs(message, new Args(), jsonSettings));
}
