using System;
using UnityEngine;
using Whaledevelop.DiContainer;
using Object = UnityEngine.Object;

namespace Whaledevelop
{
    [Serializable]
    public class GameCallbacksInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var monoBehaviourCallbacksGameObject = new GameObject(nameof(MonoBehaviourCallbacks));
            var monoBehaviourCallbacks = monoBehaviourCallbacksGameObject.AddComponent<MonoBehaviourCallbacks>();
            Object.DontDestroyOnLoad(monoBehaviourCallbacksGameObject);

            Container.Bind<IGameEventsContainer>(monoBehaviourCallbacks);
        }
    }
}