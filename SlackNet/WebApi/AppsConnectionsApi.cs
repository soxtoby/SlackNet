﻿using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IAppsConnectionsApi
{
    /// <summary>
    /// Generate a temporary Socket Mode WebSocket URL that your app can connect to in order to receive events and interactive payloads over.
    /// This API method uses an app-level token to generate a dynamic WebSocket URL.
    /// Use it with your app in Socket Mode to receive events and interactive feature payloads to the URL, rather than a public HTTP Request URL.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.connections.open/">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<ConnectionOpenResponse> Open(CancellationToken cancellationToken = default);
}

public class AppsConnectionsApi : IAppsConnectionsApi
{
    private readonly ISlackApiClient _client;
    public AppsConnectionsApi(ISlackApiClient client) => _client = client;

    public Task<ConnectionOpenResponse> Open(CancellationToken cancellationToken = default) =>
        _client.Post<ConnectionOpenResponse>("apps.connections.open", new Args(), cancellationToken);
}