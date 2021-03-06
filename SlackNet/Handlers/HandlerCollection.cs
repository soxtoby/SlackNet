using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SlackNet.Handlers
{
    class HandlerCollection<T> : IEnumerable<T>
    {
        private readonly List<Lazy<T>> _handlers;

        public HandlerCollection(SlackRequestContext context, IEnumerable<Func<SlackRequestContext, T>> handlerFactories) =>
            _handlers = handlerFactories
                .Select(f => new Lazy<T>(() => f(context)))
                .ToList();

        public IEnumerator<T> GetEnumerator() => _handlers.Select(h => h.Value).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}