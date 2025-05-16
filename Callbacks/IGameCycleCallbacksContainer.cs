using System;

namespace Whaledevelop
{
    public interface IGameCycleCallbacksContainer
    {
        event Action OnApplicationQuitEvent;

        // event Action OnDrawGizmosEvent;
    }
}