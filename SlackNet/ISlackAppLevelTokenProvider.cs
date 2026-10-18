using System.Threading;
using System.Threading.Tasks;

namespace SlackNet;

/// <summary>
/// Supplies the app-level token used to open a Socket Mode connection.
/// Use this to select credentials dynamically, such as for a multi-app host.
/// </summary>
public interface ISlackAppLevelTokenProvider
{
    /// <summary>
    /// Gets the app-level token that should authorize the specified request.
    /// </summary>
    Task<string> GetAppLevelToken(SlackApiCallContext context, CancellationToken cancellationToken);
}
