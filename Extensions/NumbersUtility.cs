using System;

namespace Whaledevelop.Extensions
{
    public static class NumbersUtility
    {
        private const float FLOAT_TOLERANCE = 0.001f;

        public static string GetCompareString(float a, float b)
        {
            return a > b ? ">" : Math.Abs(a - b) < FLOAT_TOLERANCE ? "==" : "<";
        }

        public static string GetCompareString(int a, int b)
        {
            return a > b ? ">" : a == b ? "==" : "<";
        }
    }
}