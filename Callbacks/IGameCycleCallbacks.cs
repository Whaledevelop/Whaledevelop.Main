using System;

namespace Whaledevelop
{
    public interface IGameCycleCallbacks
    {
        event Action OnApplicationQuitEvent;

        event Action OnDrawGizmosEvent;
    }
}