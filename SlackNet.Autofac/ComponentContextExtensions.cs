using Autofac;

namespace SlackNet.Autofac
{
    public static class ComponentContextExtensions
    {
        public static ISlackServiceProvider SlackServices(this IComponentContext container) => container.Resolve<ISlackServiceProvider>();
    }
}