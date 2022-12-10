using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackNet.Handlers;

public interface IHandlerIndex<THandler>
{
    bool HasHandler(string key);
    bool TryGetHandler(string key, out THandler handler);
}

class HandlerIndex<THandler> : IHandlerIndex<THandler>
{
    private readonly Dictionary<string, Lazy<THandler>> _handlers;

    public HandlerIndex(SlackRequestContext requestContext, IReadOnlyDictionary<string, Func<SlackRequestContext, THandler>> handlers) =>
        _handlers = handlers.ToDictionary(
            kv => kv.Key,
            kv => new Lazy<THandler>(() => kv.Value(requestContext)));

    public bool HasHandler(string key) => _handlers.ContainsKey(key);

    public bool TryGetHandler(string key, out THandler handler)
    {
        if (_handlers.TryGetValue(key, out var lazyHandler))
        {
            handler = lazyHandler.Value;
            return true;
        }
        else
        {
            handler = default;
            return false;
        }
    }
}