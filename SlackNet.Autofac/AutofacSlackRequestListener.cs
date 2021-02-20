using System.Threading.Tasks;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    class AutofacSlackRequestListener : ISlackRequestListener
    {
        private readonly ILifetimeScope _rootScope;
        public AutofacSlackRequestListener(ILifetimeScope rootScope) => _rootScope = rootScope;

        public Task OnRequestBegin(SlackRequestContext context)
        {
            context.SetLifetimeScope(_rootScope.BeginLifetimeScope());
            return Task.CompletedTask;
        }

        public async Task OnRequestEnd(SlackRequestContext context) =>
            await context.LifetimeScope().DisposeAsync().ConfigureAwait(false);
    }
}