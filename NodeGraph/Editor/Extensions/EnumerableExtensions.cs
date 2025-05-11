using System;
using System.Collections.Generic;
using System.Linq;
using Whaledevelop.Scopes;

namespace Whaledevelop.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool TryGetFirst<T>(this IEnumerable<T> self, Predicate<T> predicate, out T value)
        {
            foreach (var item in self)
            {
                if (!predicate(item))
                {
                    continue;
                }

                value = item;
                return true;
            }

            value = default;
            return false;
        }
        
        public static List<T> ToList<T>(this IEnumerable<T> self, List<T> list)
        {
            return self.ToList(list, false);
        }

        public static List<T> ToList<T>(this IEnumerable<T> self, List<T> list, bool append)
        {
            if (!append)
            {
                list.Clear();
            }

            list.AddRange(self);

            return list;
        }
        
        public static IEnumerable<T> Cache<T>(this IEnumerable<T> self)
        {
            using (ListScope<T>.Create(out var items))
            {
                items.AddRange(self);

                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }
    }
}