using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SlackNet.AspNetCore;

class SocketModeService(
    ISlackSocketModeClient socketModeClient,
    ILogger<SocketModeService> logger,
    SlackSocketModeConfiguration config
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await socketModeClient.Connect(config.ConnectionOptions, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error connecting Slack socket mode client");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        socketModeClient.DisconnectAsync();
}
