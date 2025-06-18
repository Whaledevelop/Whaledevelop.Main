using System;
using System.Collections.Generic;
using UnityEngine;

namespace Whaledevelop.DiContainer
{
    [Serializable]
    public class MainContainerContext
    {
        [SerializeField]
        private List<ScriptableObjectInstaller> _scriptableObjectInstallers = new();

        [SerializeField]
        private List<MonoBehaviourInstaller> _monoBehaviourInstallers = new();

        [SerializeReference]
        private List<MonoInstaller> _monoInstallers = new();

        public IDiContainer Container { get; private set; }

        public void InstallBindings()
        {
            Container = DiContainerUtility.MainContainer;

            foreach (var installer in _scriptableObjectInstallers)
            {
                installer.Container = Container;
                installer.InstallBindings();
            }

            foreach (var installer in _monoBehaviourInstallers)
            {
                installer.Container = Container;
                installer.InstallBindings();
            }

            foreach (var installer in _monoInstallers)
            {
                installer.Container = Container;
                installer.InstallBindings();
            }
        }
    }
}