using System.Collections.Generic;

namespace SlackNet.Handlers
{
    public class SlackRequestContext
    {
        private readonly Dictionary<string, object> _items = new();

        /// <summary>
        /// Gets or sets the value with the associated key.
        /// Returns null if no value has been set for the key.
        /// </summary>
        public object this[string key]
        {
            get => _items.TryGetValue(key, out var value) ? value : default;
            set => _items[key] = value;
        }

        public bool Remove(string key) => _items.Remove(key);
        public bool ContainsKey(string key) => _items.ContainsKey(key);
        public bool TryGetValue(string key, out object value) => _items.TryGetValue(key, out value);
    }
}