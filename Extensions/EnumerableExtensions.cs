using System;
using System.Collections.Generic;
using System.Linq;
using Whaledevelop.Scopes;

namespace Whaledevelop.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> self, Predicate<T> predicate)
        {
            var index = 0;

            foreach (var element in self)
            {
                if (predicate.Invoke(element))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var element in self)
            {
                action.Invoke(element);
            }
        }

        public static void ToList<T>(this IEnumerable<T> self, ICollection<T> collection)
        {
            foreach (var element in self)
            {
                collection.Add(element);
            }
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

        public static bool TryGetFirstElement<T>(this IEnumerable<T> self, out T value)
        {
            foreach (var item in self)
            {
                value = item;
                return true;
            }

            value = default;
            return false;
        }

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

        public static bool TryGetLastElement<T>(this IEnumerable<T> self, out T value)
        {
            var found = false;
            value = default;
            foreach (var item in self)
            {
                found = true;
                value = item;
            }

            return found;
        }

        public static bool TryGetLast<T>(this IEnumerable<T> self, Predicate<T> predicate, out T value)
        {
            var found = false;
            value = default;

            foreach (var item in self)
            {
                if (!predicate(item))
                {
                    continue;
                }

                value = item;
                found = true;
            }

            return found;
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

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var elements = source.ToArray();
            for (var i = elements.Length - 1; i > 0; i--)
            {
                var swapIndex = UnityEngine.Random.Range(0, i + 1);
                (elements[i], elements[swapIndex]) = (elements[swapIndex], elements[i]);
            }
            foreach (var element in elements)
            {
                yield return element;
            }
        }
    }
}