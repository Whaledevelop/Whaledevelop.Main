using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Whaledevelop.Scopes
{
    public static class PoolUtility<T>
        where T : class, new()
    {
        private static readonly Stack<T> Values = new();

        public static void Push(T value)
        {
            Assert.IsFalse(Values.Contains(value));
            Values.Push(value);
        }

        public static T Pull()
        {
            return Values.Count > 0
                ? Values.Pop()
                : new();
        }
    }
}