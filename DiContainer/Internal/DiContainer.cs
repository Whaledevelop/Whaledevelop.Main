using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Whaledevelop.DiContainer.Internal
{
    class DiContainer : IDiInternalContainer, IDisposable
    {
        private readonly InjectableInfoContainer _injectableInfoContainer;
        private readonly Dictionary<BindKey, object> _parentBinds;

        private bool _isDisposed;

        internal HashSet<IDisposable> Disposables { get; }

        internal Dictionary<BindKey, object> Binds { get; } = new();

        public DiContainer(
            InjectableInfoContainer injectableInfoContainer,
            HashSet<IDisposable> disposables,
            Dictionary<BindKey, object> binds = null)
        {
            _injectableInfoContainer = injectableInfoContainer;
            Disposables = disposables;
            _parentBinds = binds;

            if (binds != null)
            {
                foreach (var bind in binds)
                {
                    Binds.Add(bind.Key, bind.Value);
                }
            }

            var selfKey = new BindKey(typeof(IDiInternalContainer), null);

            Binds.Remove(selfKey);
            Binds[selfKey] = this;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (var pair in Binds)
            {
                if (_parentBinds != null && _parentBinds.ContainsKey(pair.Key))
                {
                    continue;
                }

                if (pair.Value is IDisposable disposable && Disposables.Remove(disposable))
                {
                    disposable.Dispose();
                }
            }

            Binds.Clear();
        }

        private void Inject<T>(T target)
            where T : class
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed.");

            var type = target.GetType();
            var info = _injectableInfoContainer.GetInfo(type)
                       ?? (type.IsGenericType ? _injectableInfoContainer.ProcessType(type) : null);

            if (info == null)
            {
                return;
            }

            InjectFields(target, info);
            InjectProperties(target, info);
            InjectConstructor(target, info);
            InjectPostMethod(target, info);
        }

        private void InjectFields<T>(T target, InjectableTypeInfo info)
        {
            foreach (var field in info.Fields)
            {
                var key = new BindKey(field.TargetType, field.Id);

                if (Binds.TryGetValue(key, out var value))
                {
                    field.SetValue(target, value);
                }
                else if (!field.Optional)
                {
                    Debug.LogError($"[DiContainer] Failed to inject field: {field.TargetType} (id={field.Id})", target as Object);
                }
            }
        }

        private void InjectProperties<T>(T target, InjectableTypeInfo info)
        {
            foreach (var property in info.Properties)
            {
                var key = new BindKey(property.TargetType, property.Id);

                if (Binds.TryGetValue(key, out var value))
                {
                    property.SetValue(target, value);
                }
                else if (!property.Optional)
                {
                    Debug.LogError($"[DiContainer] Failed to inject property: {property.TargetType} (id={property.Id})", target as Object);
                }
            }
        }

        private void InjectConstructor<T>(T target, InjectableTypeInfo info)
        {
            if (info.ConstructMethod == null)
            {
                return;
            }

            var parameters = info.ConstructParameters;
            var args = new object[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];
                var key = new BindKey(param.Type, param.Id);

                if (Binds.TryGetValue(key, out var value))
                {
                    args[i] = value;
                }
                else
                {
                    Debug.LogError($"[DiContainer] Failed to inject Construct(...) parameter: {param.Type} (id={param.Id})", target as Object);
                    return;
                }
            }

            info.ConstructMethod.Invoke(target, args);
        }

        private void InjectPostMethod<T>(T target, InjectableTypeInfo info)
        {
            if (info.Method != null)
            {
                info.Method.Invoke(target, null);
            }
        }

        #region IDiContainer

        void IDiContainer.Inject<T>(T obj) where T : class
        {
            Inject(obj);
        }

        T IDiContainer.Resolve<T>() where T : class
        {
            var key = new BindKey(typeof(T), null);

            if (Binds.TryGetValue(key, out var result))
            {
                return result as T;
            }

            Debug.LogError($"Nothing to resolve of type {typeof(T)}");

            return default;
        }

        bool IDiContainer.TryResolve<T>(out T instance) where T : class
        {
            var key = new BindKey(typeof(T), null);

            var exist = Binds.TryGetValue(key, out var result);
            instance = exist ? result as T : default;

            return exist;
        }

        #endregion

        #region IDiInternalContainer

        void IDiInternalContainer.Bind<T>(T instance, string id)
            where T : class
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed.");

            var key = new BindKey(typeof(T), id);

            Binds[key] = instance;

            if (instance is IDisposable disposable)
            {
                Disposables.Add(disposable);
            }
        }

        void IDiInternalContainer.Bind(Type type, object instance, string id)
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed.");

            var key = new BindKey(type, id);

            Binds[key] = instance;

            if (instance is IDisposable disposable)
            {
                Disposables.Add(disposable);
            }
        }

        bool IDiInternalContainer.Unbind<T>(T instance, string id)
        {
            var key = new BindKey(typeof(T), id);

            if (!Binds.TryGetValue(key, out var value) || value != instance)
            {
                return false;
            }

            return Binds.Remove(key);
        }

        bool IDiInternalContainer.Unbind(Type type, object instance, string id)
        {
            var key = new BindKey(type, id);

            if (!Binds.TryGetValue(key, out var value) || value != instance)
            {
                return false;
            }

            return Binds.Remove(key);
        }

        bool IDiInternalContainer.IsInjectable<T>(T obj) where T : class
        {
            return _injectableInfoContainer.GetInfo(obj.GetType()) != null;
        }

        void IDiInternalContainer.InjectGameObject(GameObject gameObject)
        {
            var behaviours = gameObject.GetComponentsInChildren<InjectableMonoBehaviour>(true);

            for (int i = 0; i < behaviours.Length; i++)
            {
                Inject(behaviours[i]);
            }
        }

        T IDiInternalContainer.Resolve<T>(string id) where T : class
        {
            var key = new BindKey(typeof(T), id);

            if (Binds.TryGetValue(key, out var result))
            {
                return result as T;
            }

            Debug.LogError($"Nothing to resolve of type {typeof(T)} and id = {id}");

            return default;
        }

        bool IDiInternalContainer.TryResolve<T>(out T result, string id) where T : class
        {
            var key = new BindKey(typeof(T), id);

            var exist = Binds.TryGetValue(key, out var value);
            result = exist ? value as T : default;

            return exist;
        }

        #endregion
    }

    readonly struct BindKey
    {
        private readonly Type _type;
        private readonly string _id;

        public BindKey(Type type, string id)
        {
            _type = type;
            _id = id;
        }
    }
}
