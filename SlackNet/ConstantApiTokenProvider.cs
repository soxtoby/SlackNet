using System.Threading;
using System.Threading.Tasks;

namespace SlackNet;

class ConstantApiTokenProvider(string token) : ISlackApiTokenProvider
{
    public Task<string> GetApiToken(SlackApiCallContext context, CancellationToken cancellationToken) => Task.FromResult(token);
}
