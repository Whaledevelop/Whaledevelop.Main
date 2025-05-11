using UnityEditor.Experimental.GraphView;

namespace Whaledevelop.Extensions
{
    public static class PortExtensions
    {
        public static void Destroy(this Port self)
        {
            foreach (var edge in self.connections.Cache())
            {
                edge.Destroy();
            }

            self.RemoveFromHierarchy();
        }
    }
}