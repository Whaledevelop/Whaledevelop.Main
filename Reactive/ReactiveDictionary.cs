using System;
using System.Collections;
using System.Collections.Generic;

namespace Whaledevelop.Reactive
{
    public class ReactiveDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new();

        public event Action<TKey, TValue> OnItemAdded;
        public event Action<TKey, TValue> OnItemUpdated;
        public event Action<TKey, TValue> OnItemRemoved;
        public event Action OnCleared;
        public event Action OnCollectionChanged;

        public int Count => _dictionary.Count;

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => Set(key, value);
        }

        public ReactiveDictionary()
        {
        }

        public ReactiveDictionary(IDictionary<TKey, TValue> initialData)
        {
            foreach (var pair in initialData)
            {
                _dictionary[pair.Key] = pair.Value;
            }
        }

        public void Set(TKey key, TValue value)
        {
            if (_dictionary.TryGetValue(key, out var existing))
            {
                if (!Equals(existing, value))
                {
                    _dictionary[key] = value;
                    OnItemUpdated?.Invoke(key, value);
                    OnCollectionChanged?.Invoke();
                }
            }
            else
            {
                _dictionary[key] = value;
                OnItemAdded?.Invoke(key, value);
                OnCollectionChanged?.Invoke();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                var removed = _dictionary.Remove(key);

                if (removed)
                {
                    OnItemRemoved?.Invoke(key, value);
                    OnCollectionChanged?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            _dictionary.Clear();

            OnCleared?.Invoke();
            OnCollectionChanged?.Invoke();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable SubscribeAdded(Action<TKey, TValue> callback)
        {
            OnItemAdded += callback;

            return new Subscription(() => OnItemAdded -= callback);
        }

        public IDisposable SubscribeRemoved(Action<TKey, TValue> callback)
        {
            OnItemRemoved += callback;

            return new Subscription(() => OnItemRemoved -= callback);
        }

        public IDisposable SubscribeUpdated(Action<TKey, TValue> callback)
        {
            OnItemUpdated += callback;

            return new Subscription(() => OnItemUpdated -= callback);
        }

        public IDisposable SubscribeCleared(Action callback)
        {
            OnCleared += callback;

            return new Subscription(() => OnCleared -= callback);
        }

        public IDisposable SubscribeChanged(Action callback)
        {
            OnCollectionChanged += callback;

            return new Subscription(() => OnCollectionChanged -= callback);
        }

        private class Subscription : IDisposable
        {
            private Action _onDispose;
            private bool _disposed;

            public Subscription(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _onDispose?.Invoke();
                _onDispose = null;
            }
        }
    }
}
