using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNet;

[Obsolete("Configure the HttpClient SlackNet uses instead, with UseHttpClient, or a SlackApiClient constructor that takes a Func<HttpClient>.")]
public interface IHttp
{
    /// <summary>
    /// Sends a request, and deserializes its JSON response.
    /// </summary>
    /// <returns>The deserialized response, or the default value of <typeparamref name="T"/> if the response isn't JSON.</returns>
    Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);
}

class Http(Func<HttpClient> getHttpClient, SlackJsonSettings jsonSettings, ILogger logger) : IHttp
{
    private readonly ILogger _log = logger.ForSource<Http>();

    public async Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        using var response = await Send(requestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);

        return IsJson(response)
            ? await Deserialize<T>(response).ConfigureAwait(false)
            : default;
    }

    /// <summary>
    /// Sends a request, and returns a successful response as soon as its headers have been read,
    /// so that its content, such as a file, can be streamed.
    /// The caller is responsible for disposing the response.
    /// </summary>
    public Task<HttpResponseMessage> ExecuteStreaming(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) =>
        Send(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    /// <summary>
    /// Sends a request, logs its response, and returns it if it was successful.
    /// When only the response headers are read, content that isn't JSON isn't logged, so that it can be streamed.
    /// </summary>
    private async Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;

        var requestLog = requestMessage.Content is null
            ? _log
            : _log.WithContext("RequestBody", await requestMessage.Content.ReadAsStringAsync().ConfigureAwait(false));

        try
        {
            response = await getHttpClient().SendAsync(requestMessage, completionOption, cancellationToken).ConfigureAwait(false);
            var responseLog = requestLog
                .WithContext("ResponseStatus", response.StatusCode)
                .WithContext("ResponseReason", response.ReasonPhrase)
                .WithContext("ResponseHeaders", response.Headers);
            (completionOption == HttpCompletionOption.ResponseHeadersRead && !IsJson(response)
                    ? responseLog // Logging a streamed body would load all of it into memory
                    : responseLog.WithContext("ResponseBody", await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)))
                .Data("Sent {RequestMethod} request to {RequestUrl}", requestMessage.Method, requestMessage.RequestUri);
        }
        catch (Exception e)
        {
            response?.Dispose(); // Also aborts reads that can only stop being awaited when cancelled, such as on .NET Standard 2.0
            requestLog.Error(e, "Error sending {RequestMethod} request to {RequestUrl}", requestMessage.Method, requestMessage.RequestUri);
            throw;
        }

        if (!response.IsSuccessStatusCode)
            response.Dispose(); // Release the connection, since the response won't be returned
        if ((int)response.StatusCode == 429) // TODO use the enum when TooManyRequests becomes available
            throw new SlackRateLimitException(response.Headers.RetryAfter.Delta);
        response.EnsureSuccessStatusCode();

        return response;
    }

    private static bool IsJson(HttpResponseMessage response) =>
        response.Content.Headers.ContentType?.MediaType == "application/json";

    private async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        using var jsonTextReader = new JsonTextReader(new StreamReader(await response.Content.ReadAsStreamAsync().ConfigureAwait(false)));
        return JsonSerializer.Create(jsonSettings.SerializerSettings).Deserialize<T>(jsonTextReader);
    }
}

public class SlackRateLimitException(TimeSpan? retryAfter) : Exception
{
    public TimeSpan? RetryAfter { get; } = retryAfter;
}
