using System;
using System.Collections;
using System.Collections.Generic;

namespace Whaledevelop
{
    public class DisposableList : IList<IDisposable>, IDisposable
    {
        private readonly List<IDisposable> _list = new();

        public void Dispose()
        {
            DisposeAll();
            _list.Clear();
        }

        private void DisposeAll()
        {
            foreach (var disposable in _list)
            {
                disposable?.Dispose();
            }
        }

        public void Add(IDisposable item)
        {
            _list.Add(item);
        }

        public bool Remove(IDisposable item)
        {
            return _list.Remove(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(IDisposable item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public int IndexOf(IDisposable item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, IDisposable item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public IDisposable this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}