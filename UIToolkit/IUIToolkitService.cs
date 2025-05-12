using Whaledevelop.Services;
using Whaledevelop.UIToolkit;

namespace Whaledevelop.UI
{
    public interface IUIToolkitService : IService
    {
        void OpenView(UIController uiController, IUIControllerModel model);

        void CloseView(UIController uiController);
    }
}