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
    }
}