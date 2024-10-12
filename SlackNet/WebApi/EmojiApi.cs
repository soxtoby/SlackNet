using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IEmojiApi
{
    /// <summary>
    /// Lists the custom emoji for a team.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/emoji.list">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A map of name/url pairs, one for each custom emoji used by the team.
    /// The alias: pseudo-protocol will be used where the emoji is an alias,
    /// the string following the colon is the name of the other emoji this emoji is an alias to.
    /// </returns>
    Task<IReadOnlyDictionary<string, string>> List(CancellationToken cancellationToken = default);
}

public class EmojiApi(ISlackApiClient client) : IEmojiApi
{
    public async Task<IReadOnlyDictionary<string, string>> List(CancellationToken cancellationToken = default) =>
        (await client.Get<EmojiResponse>("emoji.list", new Args(), cancellationToken).ConfigureAwait(false)).Emoji;
}