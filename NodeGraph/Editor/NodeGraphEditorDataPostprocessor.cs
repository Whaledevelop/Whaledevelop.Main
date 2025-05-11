using System;
using UnityEditor;

namespace Whaledevelop.NodeGraph
{
    public class NodeGraphEditorDataPostprocessor : AssetPostprocessor
    {
        public static event Action<string> NodeGraphEditorDataImported;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var assetPath in importedAssets)
            {
                var importedType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                if (typeof(NodeGraphEditorData).IsAssignableFrom(importedType))
                {
                    NodeGraphEditorDataImported?.Invoke(assetPath);
                }
            }
        }
    }
}