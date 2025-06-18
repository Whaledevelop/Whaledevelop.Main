#if UNITY_EDITOR
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine.UIElements;

namespace Whaledevelop.UIToolkit
{
    public static class UxmlUtility
    {
        public static List<string> ExtractNames(VisualTreeAsset visualTree)
        {
            if (!visualTree)
            {
                return new List<string>();
            }

            var path = AssetDatabase.GetAssetPath(visualTree);
            if (string.IsNullOrEmpty(path))
            {
                return new List<string>();
            }

            var doc = new XmlDocument();
            doc.Load(path);

            var result = new HashSet<string>();
            var elements = doc.GetElementsByTagName("*");

            foreach (XmlNode node in elements)
            {
                var nameAttr = node.Attributes?["name"];
                if (nameAttr != null)
                {
                    result.Add(nameAttr.Value);
                }
            }

            return new List<string>(result);
        }
    }
}
#endif