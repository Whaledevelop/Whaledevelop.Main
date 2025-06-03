using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.GameSystems;

namespace Whaledevelop.GameStates
{
    [Serializable]
    public class GameSystemsGameTransition : IGameTransition
    {
        [SerializeField] 
        private bool _keepDuplicateSystems = true;

        [Inject] private IGameSystemsService _gameSystemsService;
        
        public async UniTask BeginAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken)
        {
            if (fromState == null)
            {
                return;
            }
            foreach (var system in fromState.RequiredGameSystems)
            {
                if (_keepDuplicateSystems && toState.RequiredGameSystems.Contains(system))
                {
                    continue;
                }
                await _gameSystemsService.RemoveSystemAsync(system, cancellationToken);
            }
        }

        public async UniTask EndAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken)
        {
            foreach (var system in toState.RequiredGameSystems)
            {
                if (_keepDuplicateSystems && fromState != null && fromState.RequiredGameSystems.Contains(system))
                {
                    continue;
                }
                await _gameSystemsService.AddSystemAsync(system, cancellationToken);
            }
        }
    }
}