using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Whaledevelop.Extensions
{
    public static class UnityObjectExtensions
    {
        public static bool IsMissingOrNotSpecified(this Object self)
        {
            return self == null || self.Equals(null);
        }

        public static string GetAssetGuid(this Object self)
        {
            Assert.IsNotNull(self);

            var assetPath = self.GetAssetPath();
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);

            Assert.IsFalse(string.IsNullOrWhiteSpace(assetGuid));

            return assetGuid;
        }

        public static string GetAssetPath(this Object self)
        {
            Assert.IsNotNull(self);

            var assetPath = AssetDatabase.GetAssetPath(self);

            Assert.IsFalse(string.IsNullOrWhiteSpace(assetPath));

            return assetPath;
        }

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