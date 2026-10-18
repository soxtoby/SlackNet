using System.Threading;
using System.Threading.Tasks;

namespace SlackNet.AspNetCore;

/// <summary>
/// Supplies the signing secret used to verify an incoming Slack request.
/// Use this to choose a secret from request context when hosting more than one Slack app.
/// </summary>
public interface ISlackSigningSecretProvider
{
    /// <summary>
    /// Gets the signing secret that should verify the specified request.
    /// </summary>
    Task<string> GetSigningSecret(SlackRequestContext context, CancellationToken cancellationToken);
}

class UnconfiguredSigningSecretProvider : ISlackSigningSecretProvider
{
    public Task<string> GetSigningSecret(SlackRequestContext context, CancellationToken cancellationToken) =>
        Task.FromResult<string>(null);
}