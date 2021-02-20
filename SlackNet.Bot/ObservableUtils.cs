using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNet.Bot
{
    static class ObservableUtils
    {
        public static IObservable<T> LimitFrequency<T>(this IObservable<T> source, TimeSpan minFrequency, Func<T, CancellationToken> getCancellationToken, IScheduler scheduler = null) =>
            new AnonymousObservable<T>(observer => new LosslessThrottlingSubscription<T>(source, observer, minFrequency, getCancellationToken, scheduler));

        public static Task<T> FirstOrDefaultAsync<T>(this IEnumerable<Task<T>> tasks, Func<T, bool> predicate) where T : class =>
            FirstOrDefaultAsync<T, T>(tasks, predicate);

        public static Task<TDerived> FirstOrDefaultAsync<TBase, TDerived>(this IEnumerable<Task<TBase>> tasks, Func<TDerived, bool> predicate) where TDerived : TBase where TBase : class =>
            tasks.ToObservable()
                .SelectMany(i => i)
                .OfType<TDerived>()
                .FirstOrDefaultAsync(predicate)
                .ToTask();
    }

    class LosslessThrottlingSubscription<T> : IDisposable
    {
        private readonly IObserver<T> _observer;
        private readonly TimeSpan _minFrequency;
        private readonly Func<T, CancellationToken> _getCancellationToken;
        private readonly IScheduler _scheduler;
        private readonly IDisposable _subscription;
        private readonly Queue<T> _buffered = new();
        private bool _done;
        private DateTimeOffset _lastEmitted = DateTimeOffset.MinValue;

        public LosslessThrottlingSubscription(IObservable<T> source, IObserver<T> observer, TimeSpan minFrequency, Func<T, CancellationToken> getCancellationToken, IScheduler scheduler)
        {
            _observer = observer;
            _minFrequency = minFrequency;
            _getCancellationToken = getCancellationToken;
            _scheduler = scheduler ?? Scheduler.Default;
            _subscription = source.Subscribe(OnNext, OnError, OnComplete);
        }

        private void OnNext(T value)
        {
            if (_scheduler.Now - _lastEmitted > _minFrequency)
            {
                Emit(value);
            }
            else
            {
                _buffered.Enqueue(value);
                if (_buffered.Count == 1)
                    ScheduleNextEmit();
            }
        }

        private void ScheduleNextEmit()
        {
            var timeSpan = _lastEmitted + _minFrequency - _scheduler.Now;
            _scheduler.Schedule(0, timeSpan, (_, _) =>
                {
                    T value;
                    do
                    {
                        value = _buffered.Dequeue();
                    } while (_getCancellationToken(value).IsCancellationRequested);

                    Emit(value);

                    if (_buffered.Count > 0)
                        ScheduleNextEmit();
                });
        }

        private void OnError(Exception e) => _observer.OnError(e);

        private void OnComplete()
        {
            _done = true;
            if (_buffered.Count == 0)
                _observer.OnCompleted();
        }

        private void Emit(T value)
        {
            _observer.OnNext(value);
            _lastEmitted = _scheduler.Now;

            if (_done && _buffered.Count == 0)
                _observer.OnCompleted();
        }

        public void Dispose() => _subscription.Dispose();
    }
}