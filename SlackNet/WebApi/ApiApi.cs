using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IApiApi
    {
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
        Task<IReadOnlyDictionary<string, string>> Test(string error, Args args, CancellationToken? cancellationToken = null);
    }

    public class ApiApi : IApiApi
    {
        private readonly SlackApiClient _client;
        public ApiApi(SlackApiClient client) => _client = client;

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