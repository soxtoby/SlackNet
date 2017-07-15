using System;
using System.Reactive.Linq;
using SlackNet.Events;

namespace SlackNet.Bot
{
    public static class IncomingMiddleware
    {
        /// <summary>
        /// Ignores all message subtypes (e.g. channel joins, message edits etc.)
        /// </summary>
        public static IObservable<IMessage> PlainMessagesOnly(IObservable<IMessage> messages) => messages.Where(m => m.RawMessage.GetType() == typeof(MessageEvent));
        /// <summary>
        /// Ignores edited messages.
        /// </summary>
        public static IObservable<IMessage> IgnoreEdits(IObservable<IMessage> messages) => messages.Where(m => !(m.RawMessage is MessageChanged));
    }
}