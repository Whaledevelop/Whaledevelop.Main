using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Whaledevelop.Rendering
{
    [System.Serializable]
    public class ScreenSpaceOutlineSettings {

        [SerializeField] 
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
    
        [SerializeField] 
        public LayerMask outlinesLayerMask;
        
        [Header("General Outline Settings")]
        public Color outlineColor = Color.black;
        [Range(0.0f, 20.0f)]
        public float outlineScale = 1.0f;
        
        [Header("Depth Settings")]
        [Range(0.0f, 100.0f)]
        public float depthThreshold = 1.5f;
        [Range(0.0f, 500.0f)]
        public float robertsCrossMultiplier = 100.0f;

        [Header("Normal Settings")]
        [Range(0.0f, 1.0f)]
        public float normalThreshold = 0.4f;

        [Header("Depth Normal Relation Settings")]
        [Range(0.0f, 2.0f)]
        public float steepAngleThreshold = 0.2f;
        [Range(0.0f, 500.0f)]
        public float steepAngleMultiplier = 25.0f;
        
        [Header("General Scene View Space Normal Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits;
        public FilterMode filterMode;
        public Color backgroundColor = Color.clear;

        [Header("View Space Normal Texture Object Draw Settings")]
        public PerObjectData perObjectData;
        public bool enableDynamicBatching;
        public bool enableInstancing;
    }
}