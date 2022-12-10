using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace SlackNet.SimpleInjector;

class SimpleInjectorSlackRequestListener : ISlackRequestListener
{
    private readonly Container _container;
    public SimpleInjectorSlackRequestListener(Container container) => _container = container;

    public void OnRequestBegin(SlackRequestContext context)
    {
        var scope = AsyncScopedLifestyle.BeginScope(_container);
        context.SetContainerScope(scope);
        context.OnComplete(() => scope.DisposeScopeAsync());
    }
}