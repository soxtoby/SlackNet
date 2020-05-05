using System;
using System.Reactive.Subjects;

namespace SlackNet.AspNetCore
{
    class SyncedSubject<T> : ISubject<T>, IDisposable
    {
        private readonly Subject<T> _inner = new Subject<T>();
        private readonly ISubject<T> _synced;
        public SyncedSubject() => _synced = Subject.Synchronize(_inner);

        public void OnCompleted() => _synced.OnCompleted();
        public void OnError(Exception error) => _synced.OnError(error);
        public void OnNext(T value) => _synced.OnNext(value);
        public IDisposable Subscribe(IObserver<T> observer) => _synced.Subscribe(observer);
        public void Dispose() => _inner.Dispose();
    }
}