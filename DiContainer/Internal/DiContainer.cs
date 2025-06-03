using System;
using System.Collections.Generic;
using System.Linq;
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

        public DiContainer(InjectableInfoContainer injectableInfoContainer, HashSet<IDisposable> disposables, Dictionary<BindKey, object> binds = null)
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

            var bindKey = new BindKey(typeof(IDiInternalContainer), null);
            Binds.Remove(bindKey);
            Binds[bindKey] = this;
        }

        internal HashSet<IDisposable> Disposables { get; }

        internal Dictionary<BindKey, object> Binds { get; } = new();

        #region IDisposable

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            foreach (var bind in Binds.Where(bind => _parentBinds == null || !_parentBinds.ContainsKey(bind.Key)))
            {
                if (bind.Value is IDisposable disposable && Disposables.Remove(disposable))
                {
                    disposable.Dispose();
                }
            }

            Binds.Clear();
        }

        #endregion

        private void Inject<T>(T @object)
            where T : class
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed. Maybe you need to re-inject correct DiContainer");

            var type = @object.GetType();
            var info = _injectableInfoContainer.GetInfo(type);

            if (info == null)
            {
                if (type.IsGenericType)
                {
                    info = _injectableInfoContainer.ProcessType(type);
                }
                else
                {
                    return;
                }
            }

            foreach (var injectableFieldInfo in info.Fields)
            {
                var bindKey = new BindKey(injectableFieldInfo.TargetType, injectableFieldInfo.Id);

                if (Binds.TryGetValue(bindKey, out var bind))
                {
                    injectableFieldInfo.SetValue(@object, bind);
                }
                else if (!injectableFieldInfo.Optional)
                {
                    Debug.LogError($"[DiContainer] Failed to inject field: {injectableFieldInfo.TargetType} (id={injectableFieldInfo.Id})", @object as Object);
                }
            }

            foreach (var injectablePropertyInfo in info.Properties)
            {
                var bindKey = new BindKey(injectablePropertyInfo.TargetType, injectablePropertyInfo.Id);

                if (Binds.TryGetValue(bindKey, out var bind))
                {
                    injectablePropertyInfo.SetValue(@object, bind);
                }
                else if (!injectablePropertyInfo.Optional)
                {
                    Debug.LogError($"[DiContainer] Failed to inject property: {injectablePropertyInfo.TargetType} (id={injectablePropertyInfo.Id})", @object as Object);
                }
            }

            if (info.ConstructMethod != null)
            {
                var paramInfos = info.ConstructParameters;
                var args = new object[paramInfos.Count];

                for (int i = 0; i < paramInfos.Count; i++)
                {
                    var bindKey = new BindKey(paramInfos[i].Type, paramInfos[i].Id);

                    if (Binds.TryGetValue(bindKey, out var dependency))
                    {
                        args[i] = dependency;
                    }
                    else
                    {
                        Debug.LogError($"[DiContainer] Failed to inject Construct(...) parameter: {paramInfos[i].Type} (id={paramInfos[i].Id})", @object as Object);
                        return;
                    }
                }

                info.ConstructMethod.Invoke(@object, args);
            }

            if (info.Method != null)
            {
                info.Method.Invoke(@object, null);
            }
        }




        #region IDiContainer

        void IDiContainer.Inject<T>(T @object)
            where T : class
        {
            Inject(@object);
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
        
        void IDiInternalContainer.Bind<T>(T instance, string id)
            where T : class
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed. Maybe you need to re inject correct DiContainer");

            Binds[new(typeof(T), id)] = instance;

            if (instance is IDisposable disposable)
            {
                Disposables.Add(disposable);
            }
        }

        void IDiInternalContainer.Bind(Type type, object instance, string id)
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed. Maybe you need to re inject correct DiContainer");

            Binds[new(type, id)] = instance;

            if (instance is IDisposable disposable)
            {
                Disposables.Add(disposable);
            }
        }

        bool IDiInternalContainer.Unbind<T>(T instance, string id)
        {
            var bindKey = new BindKey(typeof(T), id);

            if (!Binds.TryGetValue(bindKey, out var foundInstance) || foundInstance != instance)
            {
                return false;
            }

            return Binds.Remove(bindKey);
        }

        bool IDiInternalContainer.Unbind(Type type, object instance, string id)
        {
            var bindKey = new BindKey(type, id);

            if (!Binds.TryGetValue(bindKey, out var foundInstance) || foundInstance != instance)
            {
                return false;
            }

            return Binds.Remove(bindKey);
        }



        bool IDiInternalContainer.IsInjectable<T>(T @object)
            where T : class
        {
            var type = @object.GetType();
            return _injectableInfoContainer.GetInfo(type) != null;
        }

        void IDiInternalContainer.InjectGameObject(GameObject gameObject)
        {
            // Для упрощения будет происходить поиск только по InjectableMonoBehaviour
            // Для правильной реализации стоит делать поиск по MonoBehaviour с аттрибутом [Inject]
            var injectableMonoBehaviours = gameObject.GetComponentsInChildren<InjectableMonoBehaviour>(true);
            for (var i = 0; i < injectableMonoBehaviours.Length; i++)
            {
                Inject(injectableMonoBehaviours[i]);
            }
        }

        T IDiInternalContainer.Resolve<T>(string id)
            where T : class
        {
            if (Binds.TryGetValue(new(typeof(T), id), out var result))
            {
                return result as T;
            }

            Debug.LogError($"Nothing to resolve of type {typeof(T)} and id = {id}");

            return default;
        }

        bool IDiInternalContainer.TryResolve<T>(out T result, string id)
            where T : class
        {
            var exist = Binds.TryGetValue(new(typeof(T), id), out var internalResult);
            result = exist ? internalResult as T : default;
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