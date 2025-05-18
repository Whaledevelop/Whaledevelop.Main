using System;
using UnityEngine;
using UnityEngine.UIElements;
using Whaledevelop.Reactive;

namespace Whaledevelop.UIToolkit
{
    [Serializable]
    public class UxmlLabelReference : UxmlElementReference
    {
        private Label _label;
        private IDisposable _subscription;

        public void Initialize(VisualElement root, string initialValue)
        {
            _label = root.Q<Label>(ElementName);

            if (_label == null)
            {
                Debug.LogError($"[UxmlLabelReference] Label '{ElementName}' not found in visual tree.");
                return;
            }

            _label.text = initialValue;
        }

        public void Initialize(VisualElement root, ReactiveValue<string> reactiveValue)
        {
            _label = root.Q<Label>(ElementName);

            if (_label == null)
            {
                Debug.LogError($"[UxmlLabelReference] Label '{ElementName}' not found in visual tree.");
                return;
            }

            _label.text = reactiveValue.Value;

            _subscription = reactiveValue.Subscribe(newValue =>
            {
                if (_label != null)
                {
                    _label.text = newValue;
                }
            });
        }
        
        public void Initialize<T>(VisualElement root, ReactiveCollection<T> collection, int index)
        {
            _label = root.Q<Label>(ElementName);

            if (_label == null)
            {
                Debug.LogError($"[UxmlLabelReference] Label '{ElementName}' not found in visual tree.");
                return;
            }

            _label.text = index < collection.Count ? collection[index]?.ToString() : string.Empty;

            _subscription = collection.SubscribeAt(index, newValue =>
            {
                if (_label != null)
                {
                    _label.text = newValue?.ToString();
                }
            });
        }


        public void Release()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}