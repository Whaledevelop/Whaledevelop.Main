using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop.GameStates
{
    [CreateAssetMenu(menuName = "Whaledevelop/States/SystemsGameState", fileName = "NewSystemsGameState")]
    public class SystemsGameState : ScriptableGameState
    {
        [SerializeReference] 
        private GameSystemScriptable[] _systems;

        [Inject] 
        private IGameSystemsService _gameSystemsService;
        
        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            return _gameSystemsService.UpdateSystemsAsync(_systems, cancellationToken);
        }
    }
}