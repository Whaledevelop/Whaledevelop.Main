using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.Services;

namespace Whaledevelop.Services
{
    [CreateAssetMenu(menuName = "Services/GameSystemsService", fileName = "GameSystemsService")]
    public class GameSystemsService : ServiceScriptableObject, IGameSystemsService
    {
        [Inject] private IDiContainer _diContainer;
        
        private readonly List<IGameSystem> _activeGameSystems = new();

        public async UniTask UpdateSystemsAsync(IGameSystem[] gameSystems, CancellationToken cancellationToken)
        {
            var systemsToRemove = _activeGameSystems.Except(gameSystems);
            foreach (var system in systemsToRemove)
            {
                await system.ReleaseAsync(cancellationToken);
                _activeGameSystems.Remove(system);
            }
            var systemsToAdd = gameSystems.Except(_activeGameSystems);
            foreach (var system in systemsToAdd)
            {
                _diContainer.Inject(system);
                await system.InitializeAsync(cancellationToken);
                _activeGameSystems.Add(system);
            }
        }

        public async UniTask EnableSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            if (_activeGameSystems.Contains(gameSystem))
            {
                Debug.Log("Game System already active");
                return;
            }
            _diContainer.Inject(gameSystem);
            await gameSystem.InitializeAsync(cancellationToken);
            _activeGameSystems.Add(gameSystem);
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}