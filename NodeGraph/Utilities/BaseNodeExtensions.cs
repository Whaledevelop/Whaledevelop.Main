using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Whaledevelop.NodeGraph
{
    public static class BaseNodeExtensions
    {
        public static PropertyInfo GetProperty(this BaseNode self, string propertyName)
        {
            return NodeGraphUtility.GetCachedProperty(self.GetType(), propertyName);
        }
        
        public static IEnumerable<(MethodInfo methodInfo, NodeMethodAttribute nodeMethodAttribute)> GetMethodInfos(this BaseNode self)
        {
            var type = self.GetType();
            var methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            foreach (var methodInfo in methodInfos.OrderBy(info => info.Name == "Start" ? 0 : 1))
            {
                var attribute = (NodeMethodAttribute)methodInfo.GetCustomAttribute(typeof(NodeMethodAttribute));
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.MethodType.IsInterface && type.GetInterface(attribute.MethodType.Name) == null)
                {
                    continue;
                }

                yield return (methodInfo, attribute);
            }
        }

        public static IEnumerable<(PropertyInfo propertyInfo, NodePropertyAttribute nodePropertyAttribute)> GetPropertyInfos(this BaseNode self)
        {
            var propertyInfos = self.GetType().GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                var attribute = (NodePropertyAttribute)propertyInfo.GetCustomAttribute(typeof(NodePropertyAttribute));
                if (attribute == null)
                {
                    continue;
                }

                yield return (propertyInfo, attribute);
            }
        }
    }
}