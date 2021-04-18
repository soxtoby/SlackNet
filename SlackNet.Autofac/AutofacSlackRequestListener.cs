using Autofac;

namespace SlackNet.Autofac
{
    class AutofacSlackRequestListener : ISlackRequestListener
    {
        private readonly ILifetimeScope _rootScope;
        public AutofacSlackRequestListener(ILifetimeScope rootScope) => _rootScope = rootScope;

        public void OnRequestBegin(SlackRequestContext context)
        {
            var scope = _rootScope.BeginLifetimeScope();
            context.SetLifetimeScope(scope);
            context.OnComplete(() => scope.DisposeAsync().AsTask());
        }
    }
}