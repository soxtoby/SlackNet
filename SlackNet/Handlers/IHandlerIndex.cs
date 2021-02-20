using System.Collections.Generic;

namespace SlackNet.Handlers
{
    public interface IHandlerIndex<THandler>
    {
        bool TryGetValue(string key, out THandler handler);
    }

    public class HandlerDictionary<THandler> : IHandlerIndex<THandler>
    {
        private readonly IReadOnlyDictionary<string, THandler> _handlers;
        public HandlerDictionary(IReadOnlyDictionary<string, THandler> handlers) => _handlers = handlers;

        public bool TryGetValue(string key, out THandler handler) => _handlers.TryGetValue(key, out handler);
    }
}