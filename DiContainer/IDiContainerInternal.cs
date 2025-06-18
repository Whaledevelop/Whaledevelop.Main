using System;
using UnityEngine;

namespace Whaledevelop.DiContainer
{
    public interface IDiContainer
    {
        void Bind<T>(T instance, string id = null)
            where T : class;

        void Bind(Type type, object instance, string id = null);

        bool Unbind<T>(T instance, string id = null)
            where T : class;

        bool Unbind(Type type, object instance, string id = null);

        void Inject<T>(T instance)
            where T : class;

        bool IsInjectable<T>(T instance)
            where T : class;

        void InjectGameObject(GameObject gameObject);

        T Resolve<T>(string id = null)
            where T : class;

        bool TryResolve<T>(out T result, string id = null)
            where T : class;

        void ResetBindings();
    }
}