using UnityEditor;
using UnityEngine;

namespace WhaledevelopEditor.Utility
{
    public static class PrefabEditorUtility
    {
        public static T InstantiateAsPrefabInPrefab<T>(T prefab, Transform parent) where T : Object
        {
            var prefabInstance = (T)PrefabUtility.InstantiatePrefab(prefab, parent);
                    
            PrefabUtility.RecordPrefabInstancePropertyModifications(prefabInstance);
            EditorUtility.SetDirty(prefabInstance);
            EditorUtility.SetDirty(parent);
            return prefabInstance;
        }
    }
}