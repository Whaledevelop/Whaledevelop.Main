using System;
using UnityEngine;

namespace Whaledevelop
{
    public class MonoBehaviourCallbacks : MonoBehaviour, IGameEventsContainer
    {
        public event Action OnUpdate;
        
        public event Action OnLateUpdate;
        
        public event Action OnFixedUpdate;
        
        public event Action OnApplicationQuitEvent;

        public event Action OnDrawGizmosEvent;

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
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