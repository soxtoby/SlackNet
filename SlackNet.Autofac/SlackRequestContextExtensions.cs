using System;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    static class SlackRequestContextExtensions
    {
        private const string ScopeKey = "AutofacScope";

        public static void SetLifetimeScope(this SlackRequestContext context, ILifetimeScope scope) => context[ScopeKey] = scope;

        public static ILifetimeScope LifetimeScope(this SlackRequestContext context) =>
            context[ScopeKey] as ILifetimeScope
            ?? throw new InvalidOperationException("Lifetime scope missing from Slack request context");
    }
}