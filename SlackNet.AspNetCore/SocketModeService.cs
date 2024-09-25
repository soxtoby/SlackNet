using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SlackNet.AspNetCore;

class SocketModeService(
    ISlackSocketModeClient socketModeClient,
    ILogger<SocketModeService> logger,
    SlackEndpointConfiguration config
) : IHostedService
{
    private readonly bool _enabled = config.SocketMode; // Holding onto this to ensure initial state is used when stopping

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_enabled)
        {
            try
            {
                await socketModeClient.Connect(config.SocketModeConnectionOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error connecting Slack socket mode client");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_enabled)
            socketModeClient.Disconnect();
        return Task.CompletedTask;
    }
}