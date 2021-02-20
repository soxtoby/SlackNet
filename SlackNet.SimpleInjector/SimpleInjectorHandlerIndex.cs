using System;
using System.Collections.Generic;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    class SimpleInjectorHandlerIndex<THandler> : IHandlerIndex<THandler>
    {
        private readonly IDictionary<string, Func<THandler>> _handlers;
        public SimpleInjectorHandlerIndex(IDictionary<string, Func<THandler>> handlers) => _handlers = handlers;

        public bool TryGetValue(string key, out THandler handler)
        {
            if (_handlers.ContainsKey(key))
            {
                handler = _handlers[key]();
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