using System;
using UnityEngine;

namespace Whaledevelop.DiContainer
{
    public interface IDiInternalContainer : IDiContainer
    {
        void Bind<T>(T instance, string id = null)
            where T : class;

        void Bind(Type type, object instance, string id = null);

        bool Unbind<T>(T instance, string id = null)
            where T : class;

        bool Unbind(Type type, object instance, string id = null);

        bool IsInjectable<T>(T @object)
            where T : class;

        void InjectGameObject(GameObject gameObject);

        T Resolve<T>(string id)
            where T : class;

        bool TryResolve<T>(out T result, string id)
            where T : class;
    }
}