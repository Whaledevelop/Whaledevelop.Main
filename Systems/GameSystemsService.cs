using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.DiContainer;
using Whaledevelop.Services;

namespace Whaledevelop.GameSystems
{
    [Serializable]
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

        public async UniTask InitializeSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            _diContainer.Inject(gameSystem);

            await gameSystem.InitializeAsync(cancellationToken);

            _updatesDispatcher.TryRegister(gameSystem);
            _activeGameSystems.Add(gameSystem);
        }

        public async UniTask ReleaseSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            _updatesDispatcher.TryUnregister(gameSystem);
            _activeGameSystems.Remove(gameSystem);
            
            await gameSystem.ReleaseAsync(cancellationToken);
        }
    }
}
