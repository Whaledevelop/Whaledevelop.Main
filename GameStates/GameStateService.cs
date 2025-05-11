using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whaledevelop.DiContainer;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    [CreateAssetMenu(menuName = "Services/GameStateService", fileName = "GameStateService")]
    public class GameStateService : ServiceScriptableObject, IGameStateService
    {
        [Inject] private IDiContainer _diContainer;

        public IGameState CurrentState { get; private set; }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
        
        public async UniTask ChangeStateAsync(IGameState state, CancellationToken cancellationToken)
        {
            if (CurrentState != null)
            {
                await CurrentState.ReleaseAsync(cancellationToken);
            }
            
            _diContainer.Inject(state);
            
            await state.InitializeAsync(cancellationToken);

            CurrentState = state;
        }
    }
}