using System;

namespace SlackNet.Bot
{
    class ConcurrentValue<T>
    {
        private readonly object _gate = new();
        public bool HasValue { get; private set; }
        private T _value;

        public T GetOrCreateValue(Func<T> createValue)
        {
            lock (_gate)
            {
                if (!HasValue)
                {
                    _value = createValue();
                    HasValue = true;
                }
                return _value;
            }
        }

        public void Clear()
        {
            lock (_gate)
            {
                HasValue = false;
                _value = default(T);
            }
        }
    }
}