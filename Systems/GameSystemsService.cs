using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.Services;

namespace Whaledevelop.GameSystems
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameSystemsService", fileName = "GameSystemsService")]
    public class GameSystemsService : Service, IGameSystemsService
    {
        private IDiContainer _diContainer;
        private IUpdateCallbacks _updateCallbacks;

        private readonly UpdateDispatcher _updatesDispatcher = new();
        private readonly List<IGameSystem> _activeGameSystems = new();

        [Inject]
        private void Construct(IDiContainer diContainer, IUpdateCallbacks updateCallbacks)
        {
            _diContainer = diContainer;
            _updateCallbacks = updateCallbacks;
        }

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _updatesDispatcher.Initialize(_updateCallbacks);
            return UniTask.CompletedTask;
        }

        protected override async UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            _updatesDispatcher.Dispose();

            var systemsToRelease = _activeGameSystems.ToArray();
            foreach (var system in systemsToRelease)
            {
                await ReleaseSystemAsync(system, cancellationToken);
            }

            _activeGameSystems.Clear();
        }

        public UniTask AddSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            return !_activeGameSystems.Contains(gameSystem)
                ? InitializeSystemAsync(gameSystem, cancellationToken)
                : UniTask.CompletedTask;
        }

        public UniTask RemoveSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            return _activeGameSystems.Contains(gameSystem)
                ? ReleaseSystemAsync(gameSystem, cancellationToken)
                : UniTask.CompletedTask;
        }

        public async UniTask UpdateSystemsAsync(IGameSystem[] gameSystems, CancellationToken cancellationToken)
        {
            var systemsToRemove = _activeGameSystems.Except(gameSystems);
            foreach (var system in systemsToRemove)
            {
                await ReleaseSystemAsync(system, cancellationToken);
            }

            var systemsToAdd = gameSystems.Except(_activeGameSystems);
            foreach (var system in systemsToAdd)
            {
                await InitializeSystemAsync(system, cancellationToken);
            }
        }

        private async UniTask InitializeSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            _diContainer.Inject(gameSystem);

            await gameSystem.InitializeAsync(cancellationToken);

            _updatesDispatcher.Register(gameSystem);
            _activeGameSystems.Add(gameSystem);
        }

        private async UniTask ReleaseSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            _updatesDispatcher.Unregister(gameSystem);

            _activeGameSystems.Remove(gameSystem);
            await gameSystem.ReleaseAsync(cancellationToken);
        }
    }
}
