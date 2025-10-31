using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IToolingApi
{
    /// <summary>
    /// Exchanges a refresh token for a new app configuration token.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/tooling.tokens.rotate/">Slack documentation</a> for more information.</remarks>
    /// <param name="refreshToken">The <c>xoxe</c> refresh token that was issued along with the old app configuration token.</param>
    /// <param name="cancellationToken"></param>
    Task<TokenRotationResponse> RotateToken(string refreshToken, CancellationToken cancellationToken = default);
}

public class ToolingApi(ISlackApiClient client) : IToolingApi
{
    public Task<TokenRotationResponse> RotateToken(string refreshToken, CancellationToken cancellationToken = default) =>
        client.Post<TokenRotationResponse>("tooling.tokens.rotate", new Args
            {
                { "refresh_token", refreshToken }
            }, cancellationToken);
}