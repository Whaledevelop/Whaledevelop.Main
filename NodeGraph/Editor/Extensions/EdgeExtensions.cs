using UnityEditor.Experimental.GraphView;

namespace Whaledevelop.Extensions
{
    public static class EdgeExtensions
    {
        public static void Destroy(this Edge self)
        {
            self.input?.Disconnect(self);
            self.output?.Disconnect(self);
            self.RemoveFromHierarchy();
        }
    }
}