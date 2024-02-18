using System.Collections.Generic;
using UnityEngine;

namespace Whaledevelop.DiContainer
{
    public class SceneContext : MonoBehaviour
    {
        [SerializeField]
        protected List<ScriptableObjectInstaller> _scriptableObjectInstallers = new();

        [SerializeField]
        protected List<MonoBehaviourInstaller> _monoBehaviourInstallers = new();

        [SerializeReference]
        protected List<MonoInstaller> _monoInstallers = new();

        [SerializeField]
        private bool _bindOnAwake = true;

        [SerializeField]
        private bool _autoInjectScene = true;

        private void Awake()
        {
            if (!_bindOnAwake)
            {
                return;
            }

            InstallBindings();
        }

        private void OnDestroy()
        {
            var container = DiContainerUtility.GetContainerById(gameObject.scene.name);
            container.Destroy();
        }

        public void InstallBindings()
        {
            var sceneContainer = DiContainerUtility.GetOrCreateContainerById(gameObject.scene.name, DiContainerUtility.MainContainer);

            for (var i = 0; i < _scriptableObjectInstallers.Count; i++)
            {
                _scriptableObjectInstallers[i].Container = sceneContainer;
                _scriptableObjectInstallers[i].InstallBindings();
            }

            for (var i = 0; i < _monoBehaviourInstallers.Count; i++)
            {
                _monoBehaviourInstallers[i].Container = sceneContainer;
                _monoBehaviourInstallers[i].InstallBindings();
            }

            for (var i = 0; i < _monoInstallers.Count; i++)
            {
                _monoInstallers[i].Container = sceneContainer;
                _monoInstallers[i].InstallBindings();
            }

            if (_autoInjectScene)
            {
                InjectScene();
            }
        }

        public void InjectScene()
        {
            var container = DiContainerUtility.GetContainerById(gameObject.scene.name);
            var rootGameObjects = gameObject.scene.GetRootGameObjects();
            for (var i = 0; i < rootGameObjects.Length; i++)
            {
                container.InjectGameObject(rootGameObjects[i]);
            }
        }
    }
}