using Whaledevelop.DiContainer;

namespace Whaledevelop.UI
{
    public abstract class UIView : InjectableMonoBehaviour
    {
        public IUIViewModel Model;

        public virtual void Initialize()
        {
        }

        public virtual void Release()
        {
        }
    }

    public abstract class UIView<T> : UIView
        where T : IUIViewModel
    {
        protected T DerivedModel => (T)Model;
    }
}