using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Whaledevelop
{
    [Serializable]
    public class GameCallbacksInstaller : IInstallerWrapper
    {
        public void InstallBindings(IDiContainerWrapper container)
        {
            var monoBehaviourCallbacksGameObject = new GameObject(nameof(MonoBehaviourCallbacks));
            var monoBehaviourCallbacks = monoBehaviourCallbacksGameObject.AddComponent<MonoBehaviourCallbacks>();
            Object.DontDestroyOnLoad(monoBehaviourCallbacksGameObject);

            container.Bind<IGameCycleCallbacks>(monoBehaviourCallbacks);
            container.Bind<IUpdateCallbacks>(monoBehaviourCallbacks);
        }
    }
}