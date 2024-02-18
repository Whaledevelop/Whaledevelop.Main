using System;
using System.Collections.Generic;

namespace Whaledevelop.Scopes
{
    public readonly struct DictionaryScope<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, TValue> _dictionary;

        private DictionaryScope(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public static DictionaryScope<TKey, TValue> Create(out Dictionary<TKey, TValue> dictionary)
        {
            dictionary = PoolUtility<Dictionary<TKey, TValue>>.Pull();
            return new(dictionary);
        }

        #region IDisposable

        void IDisposable.Dispose()
        {
            _dictionary.Clear();
            PoolUtility<Dictionary<TKey, TValue>>.Push(_dictionary);
        }

        #endregion
    }
}