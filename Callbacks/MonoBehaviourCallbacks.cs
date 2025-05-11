using System;
using UnityEngine;

namespace Whaledevelop
{
    public class MonoBehaviourCallbacks : MonoBehaviour, IGameEventsContainer
    {
        public event Action OnUpdate;
        
        public event Action OnApplicationQuitEvent;

        public event Action OnDrawGizmosEvent;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
        
        private void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnDrawGizmosEvent?.Invoke();
        }
    }
}