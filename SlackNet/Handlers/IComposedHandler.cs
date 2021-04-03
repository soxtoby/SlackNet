using System.Collections.Generic;

namespace SlackNet.Handlers
{
    interface IComposedHandler<in TRequest>
    {
        IEnumerable<object> InnerHandlers(TRequest request);
    }

    static class ComposedHandler
    {
        public static IEnumerable<object> InnerHandlers<THandler, TRequest>(this THandler handler, TRequest request) where THandler : class =>
            handler is IComposedHandler<TRequest> composed
                ? composed.InnerHandlers(request)
                : new[] { handler };
    }
}