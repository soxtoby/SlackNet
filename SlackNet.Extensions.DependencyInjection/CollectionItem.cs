namespace SlackNet.Extensions.DependencyInjection
{
    class CollectionItem<T>
    {
        public CollectionItem(T item) => Item = item;

        public T Item { get; }
    }
}