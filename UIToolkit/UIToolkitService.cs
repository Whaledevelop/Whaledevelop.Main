using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Whaledevelop.Services;
using Whaledevelop.UI;

namespace Whaledevelop.UIToolkit
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/UIToolkitService", fileName = "UIToolkitService")]
    public class UIToolkitService : ServiceScriptableObject, IUIToolkitService
    {
        [SerializeField] private UIDocument _uiDocumentPrefab;
        
        private readonly List<UIController> _controllers = new();
        private UIDocument _uiDocument;
        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _uiDocument = Object.Instantiate(_uiDocumentPrefab);
            Object.DontDestroyOnLoad(_uiDocument);
            return UniTask.CompletedTask;
        }

        public void OpenView(UIController uiController, IUIControllerModel model)
        {
            if (_controllers.Contains(uiController))
            {
                Debug.Log("UIToolkit view already opened");
                return;
            }
            uiController.Model = model;
            uiController.Initialize();
            Debug.Log($"Open view {uiController.VisualElement}"); 
            _uiDocument.rootVisualElement.Add(uiController.VisualElement);
            _controllers.Add(uiController);
        }

        public void CloseView(UIController controller)
        {
            if (!_controllers.Contains(controller))
            {
                Debug.Log("No controller");
                return;
            }
            _uiDocument.rootVisualElement.Remove(controller.VisualElement);
            controller.Release();
            _controllers.Remove(controller);
        }
    }
}