using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet
{
    public interface IHttp
    {
        Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null);
    }

    class Http : IHttp
    {
        private readonly HttpClient _client;
        private readonly SlackJsonSettings _jsonSettings;

        public Http(HttpClient client, SlackJsonSettings jsonSettings)
        {
            _client = client;
            _jsonSettings = jsonSettings;
        }

        public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null)
        {
            var response = await _client.SendAsync(requestMessage, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);

            if ((int)response.StatusCode == 429) // TODO use the enum when TooManyRequests becomes available
                throw new SlackRateLimitException(response.Headers.RetryAfter.Delta);
            response.EnsureSuccessStatusCode();
            
            return response.Content.Headers.ContentType.MediaType == "application/json"
                ? await Deserialize<T>(response).ConfigureAwait(false)
                : default;
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response) =>
            JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<T>(new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false))));
    }

    class SlackRateLimitException : Exception
    {
        public TimeSpan? RetryAfter { get; }
        
        public SlackRateLimitException(TimeSpan? retryAfter) => RetryAfter = retryAfter;
    }
}