using Whaledevelop.Services;

namespace Whaledevelop.UI
{
    public interface IUIService : IService
    {
        bool TryGetModel<T>(out T model)
            where T : IUIViewModel;

        void OpenView(UIView viewPrefab, IUIViewModel viewModel);

        void CloseView(IUIViewModel viewModel);
    }
}