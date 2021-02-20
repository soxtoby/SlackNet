using System.Collections.Generic;
using System.Linq;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    class KeyedHandlerIndex<THandler> : IHandlerIndex<THandler>
    {
        private readonly Dictionary<string, KeyedItem<THandler>> _handlers;
        public KeyedHandlerIndex(IEnumerable<KeyedItem<THandler>> keyedItems) => _handlers = keyedItems.ToDictionary(k => k.Key);

        public bool TryGetValue(string key, out THandler handler)
        {
            if (_handlers.TryGetValue(key, out var keyedHandler))
            {
                handler = keyedHandler.Item;
                return true;
            }
            else
            {
                handler = default;
                return false;
            }
        }
    }
}