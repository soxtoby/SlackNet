namespace SlackNet.AspNetCore
{
    class KeyedItem<T>
    {
        public KeyedItem(T item, string key)
        {
            Item = item;
            Key = key;
        }

        public T Item { get; }
        public string Key { get; }
    }
}