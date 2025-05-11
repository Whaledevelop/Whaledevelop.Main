using System;

namespace Whaledevelop
{
    public interface IGameEventsContainer
    {
        event Action OnUpdate;
        
        event Action OnApplicationQuitEvent;

        event Action OnDrawGizmosEvent;
    }
}