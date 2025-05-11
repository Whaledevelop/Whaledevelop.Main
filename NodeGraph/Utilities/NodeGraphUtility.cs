using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Whaledevelop.NodeGraph
{
    public static class NodeGraphUtility
    {
        private static readonly Dictionary<(Type nodeType, string propertyName), PropertyInfo> CACHED_PROPERTIES = new(64);

        public static PropertyInfo GetCachedProperty(Type type, string propertyName)
        {
            var key = (type, propertyName);
            if (CACHED_PROPERTIES.TryGetValue(key, out var cachedValue))
            {
                return cachedValue;
            }

            var propertyInfos = type.GetProperties();
            PropertyInfo result = null;
            foreach (var propertyInfo in propertyInfos)
            {
                var attribute = (NodePropertyAttribute)propertyInfo.GetCustomAttribute(typeof(NodePropertyAttribute));
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Name != propertyName)
                {
                    continue;
                }

                result = propertyInfo;
                break;
            }

            CACHED_PROPERTIES.Add(key, result);
            return result;
        }
        
        public static string GetPortName(string text, int symbolsLimit = 30)
        {
            return !string.IsNullOrEmpty(text)
                ? text.Length > symbolsLimit ? text[..symbolsLimit] + "..." : text
                : string.Empty;
        }
        
        public static BaseNode GetConnectedNode(string guid, IEnumerable<NodeEdge> edges, IEnumerable<BaseNode> nodes, string portName)
        {
            var edge = edges.FirstOrDefault(edge => edge.OutputPortName == portName && edge.OutputNodeGuid == guid);
            return edge == null ? null : nodes.FirstOrDefault(node => node.Guid == edge.InputNodeGuid);
        }
    }
}