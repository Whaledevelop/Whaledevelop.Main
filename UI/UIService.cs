using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.Extensions;
using Whaledevelop.Services;
using Object = UnityEngine.Object;

namespace Whaledevelop.UI
{
    [Serializable]
    public class UIService : Service, IUIService
    {
        [SerializeField]
        private GameObject _canvasPrefab;

        [NonSerialized]
        private RectTransform _canvasRectTransform;

        [Inject]
        private IDiContainer _diContainer;

        private Dictionary<IUIViewModel, UIView> _views = new();

        public bool TryGetModel<T>(out T model)
            where T : IUIViewModel
        {
            if (_views.TryGetFirst(keyValue => keyValue.Key is T, out var keyValuePair))
            {
                model = (T)keyValuePair.Key;
                return true;
            }
            model = default;
            return false;
        }

        public void OpenView(UIView viewPrefab, IUIViewModel viewModel)
        {
            if (_views.ContainsKey(viewModel))
            {
                Log.Info("View already opened");
                return;
            }
            var viewInstance = Object.Instantiate(viewPrefab, _canvasRectTransform);
            _diContainer.Inject(viewInstance);
            viewInstance.Model = viewModel;
            viewInstance.Initialize();
            _views.Add(viewModel, viewInstance);
        }

        public void CloseView(IUIViewModel viewModel)
        {
            if (!_views.TryGetValue(viewModel, out var viewInstance))
            {
                Log.Info("No view instance");
                return;
            }
            viewInstance.Release();
            _views.Remove(viewModel);
            Object.Destroy(viewInstance.gameObject);
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            var canvas = Object.Instantiate(_canvasPrefab);
            Object.DontDestroyOnLoad(canvas);
            _canvasRectTransform = canvas.GetComponent<RectTransform>();

            return UniTask.CompletedTask;
        }
    }
}