using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet;

namespace SlackNetx
{
    //public interface IHttp
    //{
    //    Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null);
    //}

    public class Http : IHttp
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

            var ret= response.Content.Headers.ContentType.MediaType == "application/json"
                ? await Deserialize<T>(response).ConfigureAwait(false)
                : default;

            System.Console.Write("[SlackProxyHTTP]" + response.Content.ReadAsStringAsync().Result);


            return ret;

        }

        private async Task<T> Deserialize<T>(HttpResponseMessage response) =>
            JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<T>(new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false))));
    }
}