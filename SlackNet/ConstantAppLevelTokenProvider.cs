using System.Threading;
using System.Threading.Tasks;

namespace SlackNet;

class ConstantAppLevelTokenProvider(string token) : ISlackAppLevelTokenProvider
{
    public Task<string> GetAppLevelToken(SlackApiCallContext context, CancellationToken cancellationToken) =>
        Task.FromResult(token);
}
