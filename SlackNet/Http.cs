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
            response.EnsureSuccessStatusCode();
            return await Deserialize<T>(response).ConfigureAwait(false);
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            var reader = new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            try { return JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<T>(new JsonTextReader(reader)); }
            catch (JsonReaderException)
            {
                reader.BaseStream.Position = 0;
                var body = reader.ReadToEnd();
                if (body == "ok")
                    return default;
                throw;
            }
        }
    }
}