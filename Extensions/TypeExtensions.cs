using System;
using System.Linq;

namespace Whaledevelop.Extensions
{
    public static class TypeExtensions
    {
        public static Type GetMainType(this Type self)
        {
            return self.IsGenericType
                ? self.GenericTypeArguments[0]
                : self;
        }

        public static Type GetMainInterface(this Type self, Type derivedType = null)
        {
            var interfaces = self.GetInterfaces();

            if (interfaces.Length == 0)
            {
                return self;
            }

            if (derivedType == null)
            {
                return interfaces[0];
            }

            return interfaces.FirstOrDefault(@interface => @interface != derivedType && derivedType.IsAssignableFrom(@interface)) ?? self;
        }
    }
}