using System;
using System.Collections;
using System.Collections.Generic;

namespace Whaledevelop.Reactive
{
    public class ReactiveCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();
        private readonly Dictionary<int, List<Action<T>>> _indexSubscribers = new();

        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;
        public event Action OnCleared;
        public event Action OnCollectionChanged;

        public int Count => _items.Count;

        public T this[int index]
        {
            get => _items[index];
            set
            {
                if (!Equals(_items[index], value))
                {
                    _items[index] = value;

                    if (_indexSubscribers.TryGetValue(index, out var subs))
                    {
                        foreach (var sub in subs)
                        {
                            sub?.Invoke(value);
                        }
                    }

                    OnCollectionChanged?.Invoke();
                }
            }
        }
        
        public ReactiveCollection()
        {
            _items = new List<T>();
        }

        public ReactiveCollection(IEnumerable<T> initialItems)
        {
            _items = new List<T>(initialItems);
        }

        public void Add(T item)
        {
            _items.Add(item);

            OnItemAdded?.Invoke(item);
            OnCollectionChanged?.Invoke();
        }
        
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            var changed = false;

            foreach (var item in items)
            {
                _items.Add(item);
                OnItemAdded?.Invoke(item);
                changed = true;
            }

            if (changed)
            {
                OnCollectionChanged?.Invoke();
            }
        }

        public bool Remove(T item)
        {
            var index = _items.IndexOf(item);
            var removed = _items.Remove(item);

            if (removed)
            {
                OnItemRemoved?.Invoke(item);
                OnCollectionChanged?.Invoke();

                if (index >= 0)
                {
                    _indexSubscribers.Remove(index);
                }
            }

            return removed;
        }

        public void Clear()
        {
            _items.Clear();

            OnCleared?.Invoke();
            OnCollectionChanged?.Invoke();

            _indexSubscribers.Clear();
        }

        public IDisposable SubscribeAdded(Action<T> callback)
        {
            OnItemAdded += callback;

            return new Subscription(() => OnItemAdded -= callback);
        }

        public IDisposable SubscribeRemoved(Action<T> callback)
        {
            OnItemRemoved += callback;

            return new Subscription(() => OnItemRemoved -= callback);
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

        public IDisposable SubscribeAt(int index, Action<T> callback)
        {
            if (!_indexSubscribers.TryGetValue(index, out var list))
            {
                list = new List<Action<T>>();
                _indexSubscribers[index] = list;
            }

            list.Add(callback);
            callback?.Invoke(_items.Count > index ? _items[index] : default);

            return new Subscription(() =>
            {
                if (_indexSubscribers.TryGetValue(index, out var subs))
                {
                    subs.Remove(callback);
                    if (subs.Count == 0)
                    {
                        _indexSubscribers.Remove(index);
                    }
                }
            });
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
