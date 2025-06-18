using System;

namespace Whaledevelop.DiContainer
{
    [Serializable]
    public abstract class MonoInstaller
    {
        public IDiInternalContainer Container { get; internal set; }

        public abstract void InstallBindings();
    }
}