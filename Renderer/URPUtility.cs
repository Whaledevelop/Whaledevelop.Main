using System.Linq;
using UnityEngine.Rendering.Universal;
using Whaledevelop.Rendering;

namespace Whaledevelop.Renderer
{
    public static class URPUtility
    {
        public static T GetRendererFeature<T>(ScriptableRendererData rendererData) where T : ScriptableRendererFeature
        {
            return rendererData.rendererFeatures.FirstOrDefault(feature => feature is T) as T;
        }
    }
}