using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace WhaledevelopEditor.Utility
{
    public static class SerializedObjectUtility
    {
        public static void SetArrayProperty<T>(Object targetObject, string propertyName, T[] arrayValue)
        {
            var serializedObject = new SerializedObject(targetObject);
            var arrayProperty = serializedObject.FindProperty(propertyName);
            arrayProperty.ClearArray();
            arrayProperty.arraySize = arrayValue.Length;
            for (var i = 0; i < arrayValue.Length; i++)
            {
                var elementProperty = arrayProperty.GetArrayElementAtIndex(i);
                SetSerializedPropertyValue(elementProperty, arrayValue[i]);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        public static void AddToArrayProperty<T>(Object targetObject, string propertyName, T newValue)
        {
            var serializedObject = new SerializedObject(targetObject);
            var arrayProperty = serializedObject.FindProperty(propertyName);
            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            var elementProperty = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
            SetSerializedPropertyValue(elementProperty, newValue);
            serializedObject.ApplyModifiedProperties();
        }
        
        public static void RemoveFromArrayProperty<T>(Object targetObject, string propertyName, T valueToRemove)
        {
            var serializedObject = new SerializedObject(targetObject);
            var arrayProperty = serializedObject.FindProperty(propertyName);

            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                var elementProperty = arrayProperty.GetArrayElementAtIndex(i);
                if (CompareSerializedPropertyValue(elementProperty, valueToRemove))
                {
                    arrayProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }
        }

        public static void SetPropertiesValues<T>(Object targetObject, Dictionary<string, T> propertiesDictionary)
        {
            var serializedObject = new SerializedObject(targetObject);
            foreach (var (propertyName, propertyValue) in propertiesDictionary)
            {
                var property = serializedObject.FindProperty(propertyName);
                SetSerializedPropertyValue(property, propertyValue);
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        public static void SetPropertyValue<T>(Object targetObject, string propertyName, T propertyValue)
        {
            var serializedObject = new SerializedObject(targetObject);
            var property = serializedObject.FindProperty(propertyName);
            SetSerializedPropertyValue(property, propertyValue);
            serializedObject.ApplyModifiedProperties();
        }
        
        public static void SetObjectProperties(Object targetObject, Dictionary<string, Object> propertiesDictionary)
        {
            var serializedObject = new SerializedObject(targetObject);
            foreach (var (propertyName, propertyValue) in propertiesDictionary)
            {
                var property = serializedObject.FindProperty(propertyName);
                property.objectReferenceValue = propertyValue;
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        public static void SetSerializedPropertyValue<T>(SerializedProperty property, T value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (value is int intValue) property.intValue = intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    if (value is bool boolValue) property.boolValue = boolValue;
                    break;
                case SerializedPropertyType.Float:
                    if (value is float floatValue) property.floatValue = floatValue;
                    break;
                case SerializedPropertyType.String:
                    if (value is string stringValue) property.stringValue = stringValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    if (value is UnityEngine.Object objectValue) property.objectReferenceValue = objectValue;
                    break;
                case SerializedPropertyType.Enum:
                    if (value is Enum enumValue) property.intValue = Convert.ToInt32(enumValue);
                    break;
                case SerializedPropertyType.Generic:
                    if (value != null)
                    {
                        var valueType = value.GetType();
                        foreach (var field in valueType.GetFields())
                        {
                            var fieldValue = field.GetValue(value);
                            var fieldProperty = property.FindPropertyRelative(field.Name);
                            if (fieldProperty != null)
                            {
                                SetSerializedPropertyValue(fieldProperty, fieldValue);
                            }
                        }
                    }
                    break;
                default:
                    Debug.LogWarning($"Unsupported property type: {property.propertyType}");
                    break;
            }
        }
        
        public static bool CompareSerializedPropertyValue<T>(SerializedProperty property, T value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return value is int intValue && property.intValue == intValue;
                case SerializedPropertyType.Boolean:
                    return value is bool boolValue && property.boolValue == boolValue;
                case SerializedPropertyType.Float:
                    return value is float floatValue && Mathf.Approximately(property.floatValue, floatValue);
                case SerializedPropertyType.String:
                    return value is string stringValue && property.stringValue == stringValue;
                case SerializedPropertyType.ObjectReference:
                    return value is UnityEngine.Object objectValue && property.objectReferenceValue == objectValue;
                case SerializedPropertyType.Enum:
                    return value is Enum enumValue && property.intValue == Convert.ToInt32(enumValue);
                default:
                    Debug.LogWarning($"Unsupported property type: {property.propertyType}");
                    return false;
            }
        }
        
        public static void SetStructPropertyValue<T>(Object targetObject, string propertyName, T value) where T : struct
        {
            var serializedObject = new SerializedObject(targetObject);
            var property = serializedObject.FindProperty(propertyName);

            if (property.propertyType == SerializedPropertyType.Generic)
            {
                foreach (var field in typeof(T).GetFields())
                {
                    var fieldProperty = property.FindPropertyRelative(field.Name);
                    if (fieldProperty == null)
                    {
                        Debug.LogWarning($"Property {field.Name} not found in serialized property.");
                        continue;
                    }

                    object fieldValue = field.GetValue(value);
                    if (fieldValue is int)
                    {
                        fieldProperty.intValue = (int)fieldValue;
                    }
                    else if (fieldValue is float)
                    {
                        fieldProperty.floatValue = (float)fieldValue;
                    }
                    else if (fieldValue is bool)
                    {
                        fieldProperty.boolValue = (bool)fieldValue;
                    }
                    else if (fieldValue is string)
                    {
                        fieldProperty.stringValue = (string)fieldValue;
                    }
                    // Add more types as necessary
                }
            }
            else
            {
                Debug.LogWarning($"Property type for {propertyName} is not Generic.");
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static T GetSerializedPropertyValue<T>(SerializedObject serializedObject, string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            return GetSerializedPropertyValue<T>(property);
        }
        
        public static T GetPropertyValue<T>(Object target, string propertyName)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);
            return GetSerializedPropertyValue<T>(property);
        }
        
        public static T GetSerializedPropertyValue<T>(SerializedProperty property)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)property.intValue;
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(object)property.floatValue;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)property.stringValue;
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)(object)property.boolValue;
            }

            if (typeof(T).IsEnum)
            {
                return (T)(object)property.enumValueIndex;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)property.objectReferenceValue;
            }

            throw new System.NotImplementedException($"Getting the property value for type {typeof(T)} is not implemented.");
        }
    }
}