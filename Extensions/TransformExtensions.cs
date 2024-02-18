using System.Collections.Generic;
using UnityEngine;
using Whaledevelop.Scopes;

namespace Whaledevelop.Extensions
{
    public static class TransformExtensions
    {
        public static void ResetToTransform(this Transform self, Transform target)
        {
            var transform = self.transform;
            transform.position = target.position;
            transform.rotation = target.rotation;
            transform.localScale = target.localScale;
        }

        public static IEnumerable<T> GetAllChildrenComponents<T>(this Transform self, bool includeNested = true)
            where T : Component
        {
            using (ListScope<T>.Create(out var components))
            {
                self.GetAllChildrenComponents(components, includeNested);
                return components.ToArray();
            }
        }

        private static void GetAllChildrenComponents<T>(this Transform self, ICollection<T> components, bool includeNested = true)
            where T : Component
        {
            foreach (Transform child in self)
            {
                if (child.TryGetComponent<T>(out var component))
                {
                    components.Add(component);
                }
                if (includeNested && child.childCount > 0)
                {
                    child.GetAllChildrenComponents(components);
                }
            }
        }
    }
}