using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.WebApi.Responses;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class EmojiApi
    {
        private readonly SlackApiClient _client;
        public EmojiApi(SlackApiClient client) => _client = client;

        /// <summary>
        /// Lists the custom emoji for a team.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A map of name/url pairs, one for each custom emoji used by the team. 
        /// The alias: pseudo-protocol will be used where the emoji is an alias,
        /// the string following the colon is the name of the other emoji this emoji is an alias to.
        /// </returns>
        public async Task<IReadOnlyDictionary<string, string>> List(CancellationToken? cancellationToken = null) =>
            (await _client.Get<EmojiResponse>("emoji.list", new Args(), cancellationToken).ConfigureAwait(false)).Emoji;
    }
}