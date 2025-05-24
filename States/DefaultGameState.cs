using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop;
using Whaledevelop.GameStates;

namespace START_Project
{
    [CreateAssetMenu(fileName = "DefaultGameState", menuName = "Whaledevelop/States/DefaultGameState")]
    public class DefaultGameState : ScriptableGameState
    {
        [SerializeField] 
        private GameSystemsConfig _gameSystemsConfig;
        
        [Inject]
        private IDiContainer _diContainer;
        
        private GameSystemsContainer _gameSystemsContainer;
        
        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _gameSystemsContainer = new GameSystemsContainer(_gameSystemsConfig);
            _diContainer.Inject(_gameSystemsContainer);
            return ((IInitializable)_gameSystemsContainer).InitializeAsync(cancellationToken);
        }

        protected override UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            return ((IInitializable)_gameSystemsContainer).ReleaseAsync(cancellationToken);
        }
    }
}