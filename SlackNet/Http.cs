using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet
{
    public interface IHttp
    {
        Task<T> Get<T>(string url, CancellationToken cancellationToken);
        Task<T> Post<T>(string url, string bodyName, HttpContent bodyContent, CancellationToken cancellationToken);
    }

    class Http : IHttp
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerSettings _serializerSettings;

        public Http(HttpClient client, JsonSerializerSettings serializerSettings)
        {
            _client = client;
            _serializerSettings = serializerSettings;
        }

        public async Task<T> Get<T>(string url, CancellationToken cancellationToken)
        {
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url), cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await Deserialize<T>(response).ConfigureAwait(false);
        }

        public async Task<T> Post<T>(string url, string bodyName, HttpContent bodyContent, CancellationToken cancellationToken)
        {
            var response = await _client.PostAsync(url, new MultipartFormDataContent { { bodyContent, bodyName } }, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await Deserialize<T>(response).ConfigureAwait(false);
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response) => 
            JsonSerializer.Create(_serializerSettings).Deserialize<T>(new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false))));
    }
}