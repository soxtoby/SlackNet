using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.WebApi.Responses;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class ApiApi
    {
        private readonly WebApiClient _client;
        public ApiApi(WebApiClient client) => _client = client;

        /// <summary>
        /// Helps you test your calling code.
        /// </summary>
        /// <param name="error">Error response to return.</param>
        /// <param name="args">Example properties to return.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// The response includes any supplied arguments. 
        /// If called with an error argument an error response is returned.
        /// </returns>
        public async Task<IReadOnlyDictionary<string, string>> Test(string error, Args args, CancellationToken? cancellationToken = null)
        {
            var query = new Args(args) { ["error"] = error };
            return (await _client.Get<TestResponse>("api.test", query, cancellationToken).ConfigureAwait(false)).Args;
        }
    }
}