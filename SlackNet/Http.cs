using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet
{
    public interface IHttp
    {
        Task<T> Get<T>(string url, CancellationToken? cancellationToken = null);
        Task<T> Post<T>(string url, string bodyName, HttpContent bodyContent, CancellationToken? cancellationToken = null);
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

        public async Task<T> Get<T>(string url, CancellationToken? cancellationToken = null)
        {
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url), cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await Deserialize<T>(response).ConfigureAwait(false);
        }

        public async Task<T> Post<T>(string url, string bodyName, HttpContent bodyContent, CancellationToken? cancellationToken = null)
        {
            var response = await _client.PostAsync(url, new MultipartFormDataContent { { bodyContent, bodyName } }, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await Deserialize<T>(response).ConfigureAwait(false);
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response) =>
            JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<T>(new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false))));
    }
}