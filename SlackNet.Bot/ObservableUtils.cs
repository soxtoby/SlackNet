using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace SlackNet.Bot
{
    static class ObservableUtils
    {
        public static IObservable<T> LimitFrequency<T>(this IObservable<T> source, TimeSpan minFrequency, IScheduler scheduler = null) =>
            new AnonymousObservable<T>(observer => new LosslessThrottlingSubscription<T>(source, observer, minFrequency, scheduler));

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
        private readonly IScheduler _scheduler;
        private readonly IDisposable _subscription;
        private bool _done;
        private int _buffered;
        private DateTimeOffset? _lastEmitted;

        public LosslessThrottlingSubscription(IObservable<T> source, IObserver<T> observer, TimeSpan minFrequency, IScheduler scheduler)
        {
            _observer = observer;
            _minFrequency = minFrequency;
            _scheduler = scheduler ?? Scheduler.Default;
            _subscription = source.Subscribe(OnNext, OnError, OnComplete);
        }

        private void OnNext(T value)
        {
            if (!_lastEmitted.HasValue
                || _scheduler.Now - _lastEmitted.Value > _minFrequency)
            {
                Emit(value);
            }
            else
            {
                _buffered++;
                var timeSpan = _lastEmitted.Value + Multiply(_minFrequency, _buffered) - _scheduler.Now;
                _scheduler.Schedule(0, timeSpan, (_, __) =>
                    {
                        _buffered--;
                        Emit(value);
                    });
            }
        }

        private static TimeSpan Multiply(TimeSpan value, int times) => TimeSpan.FromTicks(value.Ticks * times);

        private void OnError(Exception e) => _observer.OnError(e);

        private void OnComplete()
        {
            _done = true;
            if (_buffered == 0)
                _observer.OnCompleted();
        }

        private void Emit(T value)
        {
            _observer.OnNext(value);
            _lastEmitted = _scheduler.Now;

            if (_done && _buffered == 0)
                _observer.OnCompleted();
        }

        public void Dispose() => _subscription.Dispose();
    }
}