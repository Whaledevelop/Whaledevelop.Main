using System;
using UnityEngine.UIElements;

namespace Whaledevelop.UIToolkit
{
    [Serializable]
    public class UxmlButtonReference : UxmlElementReference
    {
        private Button _button;
        private Action _onClickCallback;
        
        public void Initialize(VisualElement root, Action callback)
        {
            _button = root.Q<Button>(ElementName);

            _onClickCallback = callback;

            _button.clicked += _onClickCallback;
        }
        
        public void Release()
        {
            if (_button != null && _onClickCallback != null)
            {
                _button.clicked -= _onClickCallback;
            }
        }

    }
}