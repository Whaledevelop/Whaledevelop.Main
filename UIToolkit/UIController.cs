using UnityEngine.UIElements;

namespace Whaledevelop.UIToolkit
{
    public abstract class UIController
    {
        public IUIControllerModel Model;

        public abstract void Initialize();

        public abstract void Release();
        
        public abstract VisualElement VisualElement { get; }
    }

    public abstract class UIController<T> : UIController
        where T : IUIControllerModel
    {
        protected T DerivedModel => (T)Model;
    }
}