using UnityEngine;

namespace Whaledevelop.DiContainer
{
    public abstract class MonoBehaviourInstaller : MonoBehaviour
    {
        public IDiContainer Container { get; internal set; }

        public abstract void InstallBindings();
    }
}