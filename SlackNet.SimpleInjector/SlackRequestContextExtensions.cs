using System;
using SimpleInjector;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    static class SlackRequestContextExtensions
    {
        private const string ScopeKey = "SimpleInjectorScope";

        public static void SetContainerScope(this SlackRequestContext context, Scope scope) => context[ScopeKey] = scope;

        public static Scope ContainerScope(this SlackRequestContext context) =>
            context[ScopeKey] as Scope
            ?? throw new InvalidOperationException("Container scope missing from Slack request context");
    }
}