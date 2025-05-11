using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Whaledevelop.NodeGraph
{
    [Serializable]
    public class NodeEdge
    {
        [SerializeField]
        [ReadOnly]
        private string _inputNodeGuid;

        [SerializeField]
        [ReadOnly]
        private string _inputPortName;

        [SerializeField]
        [ReadOnly]
        private string _outputNodeGuid;

        [SerializeField]
        [ReadOnly]
        private string _outputPortName;

        public string InputNodeGuid
        {
            get => _inputNodeGuid;
            set => _inputNodeGuid = value;
        }

        public string InputPortName
        {
            get => _inputPortName;
            set => _inputPortName = value;
        }

        public string OutputNodeGuid
        {
            get => _outputNodeGuid;
            set => _outputNodeGuid = value;
        }

        public string OutputPortName
        {
            get => _outputPortName;
            set => _outputPortName = value;
        }

        public NodeEdge Copy()
        {
            return new NodeEdge()
            {
                _inputNodeGuid = _inputNodeGuid,
                _inputPortName = _inputPortName,
                _outputNodeGuid = _outputNodeGuid,
                _outputPortName = _outputPortName
            };
        }

        public override string ToString()
        {
            return $"From guid:{_inputNodeGuid}, port:{_inputPortName}, to guid:{_inputNodeGuid}, port:{_outputPortName}";
        }
    }
}