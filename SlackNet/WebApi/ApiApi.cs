using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IApiApi
{
    /// <summary>
    /// Helps you test your calling code.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/api.test/">Slack documentation</a> for more information.</remarks>
    /// <param name="error">Error response to return.</param>
    /// <param name="args">Example properties to return.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// The response includes any supplied arguments.
    /// If called with an error argument an error response is returned.
    /// </returns>
    Task<IReadOnlyDictionary<string, string>> Test(string error, Args args, CancellationToken cancellationToken = default);
}

public class ApiApi(ISlackApiClient client) : IApiApi
{
    public async Task<IReadOnlyDictionary<string, string>> Test(string error, Args args, CancellationToken cancellationToken = default)
    {
        var query = new Args(args) { ["error"] = error };
        return (await client.Post<TestResponse>("api.test", query, cancellationToken).ConfigureAwait(false)).Args;
    }
}