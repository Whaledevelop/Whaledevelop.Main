using System;

namespace Whaledevelop
{
    public interface IUpdateCallbacksContainer
    {
        event Action OnUpdate;
        
        event Action OnLateUpdate;
        
        event Action OnFixedUpdate;
    }
}