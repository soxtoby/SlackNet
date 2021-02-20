using System.Threading.Tasks;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    class SimpleInjectorSlackRequestListener : ISlackRequestListener
    {
        private readonly Container _container;
        public SimpleInjectorSlackRequestListener(Container container) => _container = container;

        public Task OnRequestBegin(SlackRequestContext context)
        {
            context.SetContainerScope(AsyncScopedLifestyle.BeginScope(_container));
            return Task.CompletedTask;
        }

        public Task OnRequestEnd(SlackRequestContext context) =>
            context.ContainerScope().DisposeScopeAsync();
    }
}