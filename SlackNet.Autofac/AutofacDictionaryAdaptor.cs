using Autofac.Features.Indexed;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    class AutofacHandlerIndex<THandler> : IHandlerIndex<THandler>
    {
        private readonly IIndex<string, THandler> _handlers;
        public AutofacHandlerIndex(IIndex<string, THandler> handlers) => _handlers = handlers;

        public bool TryGetValue(string key, out THandler handler) => _handlers.TryGetValue(key, out handler);
    }
}