namespace SlackNet.AspNetCore
{
    class CompositeItem<T>
    {
        public CompositeItem(T item) => Item = item;

        public T Item { get; }
    }
}