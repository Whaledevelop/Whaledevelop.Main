using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Whaledevelop.DiContainer;
using Whaledevelop;
using Whaledevelop.Services;

namespace Whaledevelop.Starter
{
    public class DefaultGameStarter : MonoBehaviour
    {
        [SerializeField] 
        private MainContainerContext _mainContainerContext;
        
        [SerializeField]
        private GameState _startState;

        private void Awake()
        {
            InstallScene(Application.exitCancellationToken).Forget();
        }

        private async UniTask InstallScene(CancellationToken cancellationToken)
        {
            _mainContainerContext.InstallBindings();

            if (_mainContainerContext.Container.TryResolve<IServicesContainer>(out var servicesContainer))
            {
                await servicesContainer.InitializeAsync(cancellationToken);
            }
            else
            {
                Debug.LogError("Could not find ServicesContainer");
            }
            if (_mainContainerContext.Container.TryResolve<IGameStatesService>(out var gameStateService))
            {
                await gameStateService.ChangeStateAsync(_startState, cancellationToken);
            }
            else
            {
                Debug.LogError("Could not find DI State Service");
            }
        }
    }
}