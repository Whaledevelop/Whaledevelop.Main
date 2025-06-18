using UnityEngine;

namespace Whaledevelop.DiContainer
{
    public abstract class ScriptableObjectInstaller : ScriptableObject
    {
        public IDiContainer Container { get; internal set; }

        public abstract void InstallBindings();
    }
}