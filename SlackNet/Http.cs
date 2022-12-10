using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet;

public interface IHttp
{
    Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null);
}

class Http : IHttp
{
    private readonly Func<HttpClient> _getHttpClient;
    private readonly SlackJsonSettings _jsonSettings;
    private readonly ILogger _log;

    public Http(Func<HttpClient> getHttpClient, SlackJsonSettings jsonSettings, ILogger logger)
    {
        _getHttpClient = getHttpClient;
        _jsonSettings = jsonSettings;
        _log = logger.ForSource<Http>();
    }

    public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null)
    {
        HttpResponseMessage response;

        var requestLog = requestMessage.Content is null
            ? _log
            : _log.WithContext("RequestBody", await requestMessage.Content.ReadAsStringAsync().ConfigureAwait(false));

        try
        {
            response = await _getHttpClient().SendAsync(requestMessage, cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
            requestLog
                .WithContext("ResponseStatus", response.StatusCode)
                .WithContext("ResponseReason", response.ReasonPhrase)
                .WithContext("ResponseHeaders", response.Headers)
                .WithContext("ResponseBody", await response.Content.ReadAsStringAsync().ConfigureAwait(false))
                .Data("Sent {RequestMethod} request to {RequestUrl}", requestMessage.Method, requestMessage.RequestUri);
        }
        catch (Exception e)
        {
            requestLog.Error(e, "Error sending {RequestMethod} request to {RequestUrl}", requestMessage.Method, requestMessage.RequestUri);
            throw;
        }

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

public class SlackRateLimitException : Exception
{
    public TimeSpan? RetryAfter { get; }

    public SlackRateLimitException(TimeSpan? retryAfter) => RetryAfter = retryAfter;
}