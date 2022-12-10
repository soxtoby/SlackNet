using System;
using System.Collections.Generic;

namespace SlackNet.Extensions.DependencyInjection;

class HandlerDisposer : IDisposable
{
    private readonly List<IDisposable> _handlers = new();

    public void Add(IDisposable handler) => _handlers.Add(handler);

    public void Dispose()
    {
        foreach (var handler in _handlers)
            handler.Dispose();
    }
}