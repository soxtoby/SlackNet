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
        public static Func<IObservable<SlackMessage>, IObservable<SlackMessage>> LimitMessageFrequency(TimeSpan minFrequency, IScheduler scheduler) =>
            messages => new AnonymousObservable<SlackMessage>(observer => new LosslessThrottlingSubscription<SlackMessage>(messages, observer, minFrequency, scheduler));
    }
}