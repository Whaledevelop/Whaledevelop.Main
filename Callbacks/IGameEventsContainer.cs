using System;

namespace Whaledevelop
{
    public interface IGameEventsContainer
    {
        event Action OnUpdate;
        
        event Action OnLateUpdate;
        
        event Action OnFixedUpdate;
        
        event Action OnApplicationQuitEvent;

        event Action OnDrawGizmosEvent;
    }
}