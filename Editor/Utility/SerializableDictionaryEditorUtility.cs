using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WhaledevelopEditor.Utility
{
    public static class SerializableDictionaryEditorUtility
    {
        public static Dictionary<TKey, TValue> GetSerializableDictionaryValue<TKey, TValue>(Object targetObject, string dictionaryFieldName)
        {
            var serializedObject = new SerializedObject(targetObject);
            var entriesProperty = serializedObject.FindProperty(dictionaryFieldName + "._entries");
            var dictionary = new Dictionary<TKey, TValue>();

            if (entriesProperty == null || !entriesProperty.isArray)
            {
                Debug.LogError("The specified property is either null or not an array.");
                return dictionary;
            }

            for (var i = 0; i < entriesProperty.arraySize; i++)
            {
                var entry = entriesProperty.GetArrayElementAtIndex(i);
                var keyProperty = entry.FindPropertyRelative("_key");
                var valueProperty = entry.FindPropertyRelative("_value");

                TKey key = SerializedObjectUtility.GetSerializedPropertyValue<TKey>(keyProperty);
                TValue value = SerializedObjectUtility.GetSerializedPropertyValue<TValue>(valueProperty);

                if (!dictionary.TryAdd(key, value))  // Check to avoid duplicate keys
                {
                    Debug.LogError($"Duplicate key found: {key}. Skipped adding this entry.");
                }
            }
            return dictionary;
        }
        
        public static void SetSerializableDictionaryProperty<TKey, TValue>(Object targetObject, string dictionaryFieldName, Dictionary<TKey, TValue> newDictionary)
        {
            var serializedObject = new SerializedObject(targetObject);
            var entriesProperty = serializedObject.FindProperty(dictionaryFieldName + "._entries");

            if (entriesProperty is not { isArray: true })
            {
                return;
            }
            entriesProperty.ClearArray();
            entriesProperty.arraySize = newDictionary.Count;
            var i = 0;
            foreach (var kvp in newDictionary)
            {
                var entry = entriesProperty.GetArrayElementAtIndex(i);
                var keyProperty = entry.FindPropertyRelative("_key");
                var valueProperty = entry.FindPropertyRelative("_value");

                SerializedObjectUtility.SetSerializedPropertyValue(keyProperty, kvp.Key);
                SerializedObjectUtility.SetSerializedPropertyValue(valueProperty, kvp.Value);
                i++;
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        public static void AddToSerializableDictionary<TKey, TValue>(Object targetObject, string dictionaryFieldName, TKey key, TValue value)
        {
            var serializedObject = new SerializedObject(targetObject);
            var entriesProperty = serializedObject.FindProperty(dictionaryFieldName + "._entries");
    
            if (entriesProperty == null || !entriesProperty.isArray)
            {
                Debug.LogError("The specified property is either null or not an array.");
                return;
            }

            entriesProperty.arraySize++;
            var newEntry = entriesProperty.GetArrayElementAtIndex(entriesProperty.arraySize - 1);
            var keyProperty = newEntry.FindPropertyRelative("_key");
            var valueProperty = newEntry.FindPropertyRelative("_value");

            SerializedObjectUtility.SetSerializedPropertyValue(keyProperty, key);
            SerializedObjectUtility.SetSerializedPropertyValue(valueProperty, value);

            serializedObject.ApplyModifiedProperties();
        }

        public static void RemoveFromSerializableDictionaryProperty<TKey>(Object targetObject, string dictionaryFieldName, TKey key)
        {
            var serializedObject = new SerializedObject(targetObject);
            var entriesProperty = serializedObject.FindProperty(dictionaryFieldName + "._entries");

            if (entriesProperty == null || !entriesProperty.isArray)
            {
                Debug.LogError("The specified property is either null or not an array.");
                return;
            }

            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                var entry = entriesProperty.GetArrayElementAtIndex(i);
                var keyProperty = entry.FindPropertyRelative("_key");

                if (SerializedObjectUtility.CompareSerializedPropertyValue(keyProperty, key))
                {
                    entriesProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }
        }
    }
}