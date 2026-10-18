using System.Threading;
using System.Threading.Tasks;

namespace SlackNet.AspNetCore;

/// <summary>
/// Supplies the legacy verification token used to verify an incoming Slack request.
/// Use this to choose a token from request context when hosting more than one Slack app.
/// </summary>
public interface ISlackVerificationTokenProvider
{
    /// <summary>
    /// Gets the verification token that should verify the specified request.
    /// </summary>
    Task<string> GetVerificationToken(SlackRequestContext context, CancellationToken cancellationToken);
}

class UnconfiguredVerificationTokenProvider : ISlackVerificationTokenProvider
{
    public Task<string> GetVerificationToken(SlackRequestContext context, CancellationToken cancellationToken) =>
        Task.FromResult<string>(null);
}