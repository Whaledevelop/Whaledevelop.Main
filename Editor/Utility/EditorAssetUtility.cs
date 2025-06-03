using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Whaledevelop.Utilities
{
    public static class EditorAssetUtility
    {
        public static T GetScriptableObjectOrCreate<T>(string assetName, string defaultFolder)
            where T : ScriptableObject
        {
            var assetGuid = AssetDatabase.FindAssets($"t:{typeof(T).Name} {assetName}")
                .FirstOrDefault(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)) == assetName);

            if (assetGuid == null)
            {
                var scriptableObject = ScriptableObject.CreateInstance<T>();
                var path = $"{defaultFolder}/{assetName}.asset";
                if (!Directory.Exists(defaultFolder))
                {
                    Directory.CreateDirectory(defaultFolder);
                }
                AssetDatabase.CreateAsset(scriptableObject, path);
                AssetDatabase.Refresh();
                return scriptableObject;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return asset;
        }
    }
}