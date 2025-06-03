using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_ENABLED
using UnityEngine;

#endif

namespace Whaledevelop.DiContainer.Internal
{
    class InjectableInfoContainer
    {
        private readonly Dictionary<Type, InjectableTypeInfo> _infos = new();

        public void Initialization(Assembly[] assemblies)
        {
            _infos.Clear();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                for (var i = 0; i < types.Length; i++)
                {
                    ProcessType(types[i]);
                }
            }
        }

        public InjectableTypeInfo ProcessType(Type mainType)
        {
            var type = mainType;

            var baseType = typeof(object);
#if UNITY_ENABLED
            if (mainType.IsSubclassOf(typeof(MonoBehaviour)))
            {
                baseType = typeof(MonoBehaviour);
            }
#endif

            InjectableTypeInfo currentTypeInfoOfType = null;
            do
            {
                currentTypeInfoOfType = ProcessFields(type, currentTypeInfoOfType);
                currentTypeInfoOfType = ProcessProperties(type, currentTypeInfoOfType);
                currentTypeInfoOfType = ProcessMethods(type, currentTypeInfoOfType);
                type = type.BaseType;
            } while (type != null && type != baseType);

            if (currentTypeInfoOfType != null)
            {
                _infos.Add(mainType, currentTypeInfoOfType);
#if UNITY_ENABLED
                if (mainType.IsSubclassOf(typeof(MonoBehaviour)) && !mainType.IsSubclassOf(typeof(InjectableMonoBehaviour)))
                {
                    if (Application.isEditor)
                    {
                        Debug.LogError($"Type {mainType} has inject fields but not derived from InjectableMonoBehaviour");
                    }
                }
#endif
            }
#if UNITY_ENABLED
            else if (mainType.IsSubclassOf(typeof(InjectableMonoBehaviour)))
            {
                _infos.Add(mainType, new InjectableTypeInfo());
                if (Application.isEditor)
                {
                    Debug.Log($"Type {mainType} derived from InjectableMonoBehaviour but there is nothing to inject");
                }
            }
#endif
            return currentTypeInfoOfType;
        }

        private static InjectableTypeInfo ProcessMethods(Type type, InjectableTypeInfo currentTypeInfoOfType)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var attributes = method.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    if (attribute is not InjectAttribute)
                    {
                        continue;
                    }

                    currentTypeInfoOfType ??= new();

                    if (method.Name == "Construct" && method.ReturnType == typeof(void))
                    {
                        if (currentTypeInfoOfType.ConstructMethod != null)
                        {
                            throw new($"DiContainer supports only one [Inject] Construct method per type: {type}");
                        }

                        currentTypeInfoOfType.ConstructMethod = method;

                        var parameters = method.GetParameters();
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            currentTypeInfoOfType.ConstructParameters.Add(new(
                                parameters[j].ParameterType,
                                null // если нужно — поддержка ID позже
                            ));
                        }

                        continue;
                    }

                    if (method.ReturnType != typeof(void) || method.GetParameters().Length != 0)
                    {
                        continue;
                    }

                    if (currentTypeInfoOfType.Method != null)
                    {
                        if ((currentTypeInfoOfType.Method.IsAbstract || currentTypeInfoOfType.Method.IsVirtual)
                            && type == method.ReflectedType)
                        {
                            continue;
                        }

                        throw new($"DiContainer works only with 1 [Inject] method. Please fix type {type}");
                    }

                    currentTypeInfoOfType.Method = method;
                }
            }

            return currentTypeInfoOfType;
        }

        private static InjectableTypeInfo ProcessProperties(Type type, InjectableTypeInfo currentTypeInfoOfType)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                foreach (var attribute in properties[i].GetCustomAttributes(true))
                {
                    if (!(attribute is InjectAttribute injectable))
                    {
                        continue;
                    }

                    if (!properties[i].CanWrite)
                    {
                        throw new($"DiContainer works only with Set properties. Please fix type {type}");
                    }

                    currentTypeInfoOfType ??= new();
                    currentTypeInfoOfType.Properties.Add(new(injectable.ID, injectable.Optional, properties[i]));
                }
            }
            return currentTypeInfoOfType;
        }

        private static InjectableTypeInfo ProcessFields(Type type, InjectableTypeInfo currentTypeInfoOfType)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (var i = 0; i < fields.Length; i++)
            {
                foreach (var attribute in fields[i].GetCustomAttributes(true))
                {
                    if (!(attribute is InjectAttribute injectable))
                    {
                        continue;
                    }
                    currentTypeInfoOfType ??= new();
                    currentTypeInfoOfType.Fields.Add(new(injectable.ID, injectable.Optional, fields[i]));
                }
            }
            return currentTypeInfoOfType;
        }

        public InjectableTypeInfo GetInfo(Type type)
        {
            _infos.TryGetValue(type, out var info);
            return info;
        }
    }
}