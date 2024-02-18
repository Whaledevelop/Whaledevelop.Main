using System;

namespace Whaledevelop.DiContainer
{
    public abstract class SingletonScriptableObjectInstaller : ScriptableObjectInstaller
    {
        [NonSerialized]
        private bool _installed;

        protected abstract void OnInstallBindings();

        public override void InstallBindings()
        {
            if (_installed)
            {
                return;
            }

            OnInstallBindings();
            _installed = true;
        }
    }
}