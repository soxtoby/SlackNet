using System;
using System.Reactive.Concurrency;

namespace SlackNet.Bot
{
    class LosslessThrottlingSubscription<T>: IDisposable
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