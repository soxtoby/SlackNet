using System;
using System.Reactive;
using System.Reactive.Concurrency;
using SlackNet.WebApi;

namespace SlackNet.Bot
{
    public static class OutgoingMiddleware
    {
        /// <summary>
        /// Slows message sending down to the specified frequency.
        /// </summary>
        public static Func<IObservable<Message>, IObservable<Message>> LimitMessageFrequency(TimeSpan minFrequency, IScheduler scheduler) =>
            messages => new AnonymousObservable<Message>(observer => new LosslessThrottlingSubscription<Message>(messages, observer, minFrequency, scheduler));
    }
}