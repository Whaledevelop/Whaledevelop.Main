using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.Services;
using Object = UnityEngine.Object;

namespace Whaledevelop.UI
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/UIService", fileName = "UIService")]
    public class UIService : Service, IUIService
    {
        [SerializeField]
        private GameObject _canvasPrefab;

        [NonSerialized]
        private RectTransform _canvasRectTransform;

        private readonly Dictionary<IUIViewModel, UIView> _viewsModelsDict = new();

        public bool TryGetModel<T>(out T resultModel) where T : IUIViewModel
        {
            foreach (var (model, _) in _viewsModelsDict)
            {
                if (model is not T typedModel)
                {
                    continue;
                }
                resultModel = typedModel;
                return true;
            }
            resultModel = default;
            return false;
        }

        public void OpenView(UIView viewPrefab, IUIViewModel viewModel)
        {
            if (_viewsModelsDict.ContainsKey(viewModel))
            {
                Debug.Log("View already opened");
                return;
            }
            var viewInstance = Object.Instantiate(viewPrefab, _canvasRectTransform);
            viewInstance.Model = viewModel;
            viewInstance.Initialize();
            _viewsModelsDict.Add(viewModel, viewInstance);
        }

        public void CloseView(IUIViewModel viewModel)
        {
            if (!_viewsModelsDict.TryGetValue(viewModel, out var viewInstance))
            {
                Debug.Log("No view instance");
                return;
            }
            viewInstance.Release();
            _viewsModelsDict.Remove(viewModel);
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