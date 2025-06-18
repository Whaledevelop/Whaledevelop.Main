using Whaledevelop.Services;

namespace Whaledevelop.UIToolkit
{
    public interface IUIToolkitService : IService
    {
        void OpenView(UIController uiController, IUIControllerModel model);

        void CloseView(UIController uiController);
    }
}