using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Whaledevelop.UIToolkit
{
    [System.Serializable]
    public class UxmlElementReference
    {
        [SerializeField]
        private VisualTreeAsset _uxml;

        [ValueDropdown(nameof(GetElementNames))]
        [SerializeField]
        private string _elementName;

        public string ElementName => _elementName;

        public VisualTreeAsset Asset => _uxml;
        
#if UNITY_EDITOR
        private IEnumerable<string> GetElementNames()
        {
            return UxmlUtility.ExtractNames(_uxml);
        }
#endif
    }
}