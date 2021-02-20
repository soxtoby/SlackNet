using System;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    static class SlackRequestContextExtensions
    {
        private const string ScopeKey = "ServiceScope";

        public static void SetServiceScope(this SlackRequestContext context, IServiceScope scope) => context[ScopeKey] = scope;

        public static IServiceScope ServiceScope(this SlackRequestContext context) =>
            context[ScopeKey] as IServiceScope
            ?? throw new InvalidOperationException("Service scope missing from Slack request context");
    }
}