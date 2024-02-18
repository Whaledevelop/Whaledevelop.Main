using UnityEngine;

namespace Whaledevelop.Extensions
{
    public static class UnityObjectExtensions
    {
        public static bool IsMissingOrNotSpecified(this Object self)
        {
            return self == null || self.Equals(null);
        }
    }
}