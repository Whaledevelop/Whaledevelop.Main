using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

            foreach (var system in _activeGameSystems)
            {
                await system.ReleaseAsync(cancellationToken);
            }

            _activeGameSystems.Clear();
        }

        public async UniTask AddSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            if (_activeGameSystems.Contains(gameSystem))
            {
                return;
            }

            await InitializeSystemAsync(gameSystem, cancellationToken);
        }

        public async UniTask RemoveSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            if (!_activeGameSystems.Contains(gameSystem))
            {
                return;
            }

            await ReleaseSystemAsync(gameSystem, cancellationToken);
        }

        public async UniTask UpdateSystemsAsync(IGameSystem[] gameSystems, CancellationToken cancellationToken)
        {
            for (var i = _activeGameSystems.Count - 1; i >= 0; i--)
            {
                var current = _activeGameSystems[i];
                var stillUsed = false;
                foreach (var t in gameSystems)
                {
                    if (t != current)
                    {
                        continue;
                    }
                    stillUsed = true;
                    break;
                }
                if (!stillUsed)
                {
                    await ReleaseSystemAsync(current, cancellationToken);
                }
            }
            foreach (var candidate in gameSystems)
            {
                if (!_activeGameSystems.Contains(candidate))
                {
                    await InitializeSystemAsync(candidate, cancellationToken);
                }
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
