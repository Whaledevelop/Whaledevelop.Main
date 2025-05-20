using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Whaledevelop.Rendering
{
    public class ScreenSpaceOutlines : ScriptableRendererFeature
    {
        [SerializeField]
        private ScreenSpaceOutlineSettings _settings = new();

        private ScreenSpaceOutlinePass _pass;

        public ScreenSpaceOutlineSettings Settings => _settings;

        public override void Create()
        {
            _pass = new ScreenSpaceOutlinePass(Settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_pass == null)
            {
                Debug.LogError("[ScreenSpaceOutlines] Pass is null");
                return;
            }

            //Debug.Log("[ScreenSpaceOutlines] Enqueuing ScreenSpaceOutlinePass");
            renderer.EnqueuePass(_pass);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pass?.Release();
            }
        }
    }
}