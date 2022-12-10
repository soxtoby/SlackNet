using System;

namespace SlackNet.Extensions.DependencyInjection;

public static class SlackRequestContextExtensions
{
    private const string ProviderKey = "ServiceProvider";

    public static void SetServiceProvider(this SlackRequestContext context, IServiceProvider scopedProvider) => context[ProviderKey] = scopedProvider;

    public static IServiceProvider ServiceProvider(this SlackRequestContext context) =>
        context[ProviderKey] as IServiceProvider
        ?? throw new InvalidOperationException("Service scope missing from Slack request context");
}