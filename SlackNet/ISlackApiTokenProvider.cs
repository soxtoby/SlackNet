using System.Threading;
using System.Threading.Tasks;

namespace SlackNet;

/// <summary>
/// Supplies the token used to authorize a Slack Web API request.
/// Use this to select credentials dynamically, such as for a multi-workspace app.
/// </summary>
public interface ISlackApiTokenProvider
{
    /// <summary>
    /// Gets the token that should authorize the specified request.
    /// </summary>
    Task<string> GetApiToken(SlackApiCallContext context, CancellationToken cancellationToken);
}
