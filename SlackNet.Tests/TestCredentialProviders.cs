using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNet.Tests;

class ApiTokenProvider(Func<SlackApiCallContext, string> getApiToken) : ISlackApiTokenProvider
{
    public Task<string> GetApiToken(SlackApiCallContext context, CancellationToken cancellationToken) =>
        Task.FromResult(getApiToken(context));
}

class AppLevelTokenProvider(Func<SlackApiCallContext, string> getAppLevelToken) : ISlackAppLevelTokenProvider
{
    public Task<string> GetAppLevelToken(SlackApiCallContext context, CancellationToken cancellationToken) =>
        Task.FromResult(getAppLevelToken(context));
}
