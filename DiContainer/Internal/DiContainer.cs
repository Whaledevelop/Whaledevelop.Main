using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Whaledevelop.DiContainer.Internal
{
    class DiContainer : IDiContainer, IDisposable
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
                // ReSharper disable once UseDeconstruction
                foreach (var bind in binds)
                {
                    Binds.Add(bind.Key, bind.Value);
                }
            }

            var bindKey = new BindKey(typeof(IDiContainer), null);
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
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed. Maybe you need to re inject correct DiContainer");

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
                    //Debug.LogWarning($"Nothing to inject {@object}");
                    return;
                }
            }

            for (var i = 0; i < info.Fields.Count; i++)
            {
                var injectableFieldInfo = info.Fields[i];
                var bindKey = new BindKey(injectableFieldInfo.TargetType, injectableFieldInfo.Id);
                if (Binds.TryGetValue(bindKey, out var bind))
                {
                    injectableFieldInfo.SetValue(@object, bind);
                }
                else if (!injectableFieldInfo.Optional)
                {
                    Debug.LogError($"Failed to inject {@object}. Not found type {injectableFieldInfo.TargetType} with id = {injectableFieldInfo.Id}", @object as Object);
                }
            }

            for (var i = 0; i < info.Properties.Count; i++)
            {
                var injectablePropertyInfo = info.Properties[i];
                var bindKey = new BindKey(injectablePropertyInfo.TargetType, injectablePropertyInfo.Id);
                if (Binds.TryGetValue(bindKey, out var bind))
                {
                    injectablePropertyInfo.SetValue(@object, bind);
                }
                else if (!injectablePropertyInfo.Optional)
                {
                    Debug.LogError($"Failed to inject {@object}. Not found type {injectablePropertyInfo.TargetType} with id = {injectablePropertyInfo.Id}", @object as Object);
                }
            }

            if (info.Method != null)
            {
                info.Method.Invoke(@object, null);
            }
        }

        #region IDiContainer

        void IDiContainer.Bind<T>(T instance, string id)
            where T : class
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed. Maybe you need to re inject correct DiContainer");

            Binds[new(typeof(T), id)] = instance;

            if (instance is IDisposable disposable)
            {
                Disposables.Add(disposable);
            }
        }

        void IDiContainer.Bind(Type type, object instance, string id)
        {
            Assert.IsFalse(_isDisposed, "DiContainer already is disposed. Maybe you need to re inject correct DiContainer");

            Binds[new(type, id)] = instance;

            if (instance is IDisposable disposable)
            {
                Disposables.Add(disposable);
            }
        }

        bool IDiContainer.Unbind<T>(T instance, string id)
        {
            var bindKey = new BindKey(typeof(T), id);

            if (!Binds.TryGetValue(bindKey, out var foundInstance) || foundInstance != instance)
            {
                return false;
            }

            return Binds.Remove(bindKey);
        }

        bool IDiContainer.Unbind(Type type, object instance, string id)
        {
            var bindKey = new BindKey(type, id);

            if (!Binds.TryGetValue(bindKey, out var foundInstance) || foundInstance != instance)
            {
                return false;
            }

            return Binds.Remove(bindKey);
        }

        void IDiContainer.Inject<T>(T @object)
            where T : class
        {
            Inject(@object);
        }

        bool IDiContainer.IsInjectable<T>(T @object)
            where T : class
        {
            var type = @object.GetType();
            return _injectableInfoContainer.GetInfo(type) != null;
        }

        void IDiContainer.InjectGameObject(GameObject gameObject)
        {
            // Для упрощения будет происходить поиск только по InjectableMonoBehaviour
            // Для правильной реализации стоит делать поиск по MonoBehaviour с аттрибутом [Inject]
            var injectableMonoBehaviours = gameObject.GetComponentsInChildren<InjectableMonoBehaviour>(true);
            for (var i = 0; i < injectableMonoBehaviours.Length; i++)
            {
                Inject(injectableMonoBehaviours[i]);
            }
        }

        T IDiContainer.Resolve<T>(string id)
            where T : class
        {
            if (Binds.TryGetValue(new(typeof(T), id), out var result))
            {
                return result as T;
            }

            Debug.LogError($"Nothing to resolve of type {typeof(T)} and id = {id}");

            return default;
        }

        bool IDiContainer.TryResolve<T>(out T result, string id)
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