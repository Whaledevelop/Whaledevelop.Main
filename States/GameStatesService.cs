using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.Services;

namespace Whaledevelop
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameStatesService", fileName = "GameStatesService")]
    public class GameStatesService : Service, IGameStatesService
    {
        [Inject] private IDiContainer _diContainer;

        public IGameState CurrentState { get; private set; }

        public async UniTask SetStateAsync(IGameState state, CancellationToken cancellationToken)
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