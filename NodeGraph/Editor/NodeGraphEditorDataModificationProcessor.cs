using System;
using UnityEditor;

namespace Whaledevelop.NodeGraph
{
    public class NodeGraphEditorDataModificationProcessor : AssetModificationProcessor
    {
        public static event Action<string> NodeGraphEditorDataWillDelete;
        public static event Action<string, string> NodeGraphEditorDataWillMove;

        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);

            if (typeof(NodeGraphEditorData).IsAssignableFrom(type))
            {
                NodeGraphEditorDataWillDelete?.Invoke(path);
            }

            return AssetDeleteResult.DidNotDelete;
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(sourcePath);

            if (typeof(NodeGraphEditorData).IsAssignableFrom(type))
            {
                NodeGraphEditorDataWillMove?.Invoke(sourcePath, destinationPath);
            }

            return AssetMoveResult.DidNotMove;
        }
    }
}