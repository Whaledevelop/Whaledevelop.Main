using System;
using System.Linq;

namespace Whaledevelop.NodeGraph
{
    public static class TypeExtensions
    {
        public static Type GetMainType(this Type self)
        {
            return self.IsGenericType
                ? self.GenericTypeArguments[0]
                : self;
        }
    }
}