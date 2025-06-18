using System;

namespace Whaledevelop.DiContainer
{
    [Serializable]
    public abstract class MonoInstaller
    {
        public IDiContainer Container { get; internal set; }

        public abstract void InstallBindings();
    }
}