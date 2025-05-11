using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Whaledevelop.Extensions
{
    public static class UnityObjectEditorExtensions
    {
        // ReSharper disable once UnusedMember.Global
        public static void SetDirty(this Object self, bool dirty)
        {
            Assert.IsNotNull(self);

            if (dirty)
            {
                EditorUtility.SetDirty(self);
            }
            else
            {
                EditorUtility.ClearDirty(self);
            }
        }

        public static void ForceSaveAsset(this Object self)
        {
            Assert.IsNotNull(self);
            EditorUtility.SetDirty(self);
            AssetDatabase.SaveAssetIfDirty(self);
        }

        public static void ForceReserializeAsset(this Object self)
        {
            var assetPath = AssetDatabase.GetAssetPath(self);
            AssetDatabase.ForceReserializeAssets(GetAssetPaths());

            IEnumerable<string> GetAssetPaths()
            {
                yield return assetPath;
            }
        }
    }
}