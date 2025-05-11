using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    [Serializable]
    public abstract class BaseNode
    {
        [SerializeField]
        [ReadOnly]
        private string _guid;

        protected BaseNode()
        {
            _guid = System.Guid.NewGuid().ToString();
        }

        public string Guid
        {
            get => _guid;
            set => _guid = value;
        }
    }
}