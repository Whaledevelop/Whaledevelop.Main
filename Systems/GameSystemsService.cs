using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Whaledevelop.Services;

namespace Whaledevelop
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameSystemsService", fileName = "GameSystemsService")]
    public class GameSystemsService : ServiceScriptableObject, IGameSystemsService, IUpdate, IFixedUpdate, ILateUpdate
    {
        [SerializeField] private GameSystemsConfig _defaultSystemsConfig;
        
        [Inject]
        private IDiContainer _diContainer;

        [Inject] 
        private IUpdateCallbacksContainer _updateCallbacksContainer;
        
        private readonly List<IUpdate> _updates = new();
        private readonly List<IFixedUpdate> _fixedUpdates = new();
        private readonly List<ILateUpdate> _lateUpdates = new();
        private readonly List<IGameSystem> _activeGameSystems = new();
        

        public async UniTask UpdateSystemsAsync(IGameSystem[] gameSystems, CancellationToken cancellationToken)
        {
            var systemsToRemove = _activeGameSystems.Except(gameSystems);
            foreach (var system in systemsToRemove)
            {
                system.RemoveFromUpdateLists(_updates, _fixedUpdates, _lateUpdates);
                await system.ReleaseAsync(cancellationToken);
                _activeGameSystems.Remove(system);
            }
            var systemsToAdd = gameSystems.Except(_activeGameSystems);
            foreach (var system in systemsToAdd)
            {
                // TODO здесь тоже сортировку надо для апдейта
                await InitializeSystemAsync(system, cancellationToken, true);
            }
        }
        
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            foreach (var system in _defaultSystemsConfig.GameSystems)
            {
                await InitializeSystemAsync(system, cancellationToken, false);
            }

            var systemsHashSet = new HashSet<GameSystemScriptable>();
            foreach (var system in _defaultSystemsConfig.UpdateOrder)
            {
                system.AddToUpdateLists(_updates, _fixedUpdates, _lateUpdates);
                systemsHashSet.Add(system);
            }
            foreach (var system in _defaultSystemsConfig.GameSystems)
            {
                if (systemsHashSet.Contains(system))
                {
                    continue;
                }
                system.AddToUpdateLists(_updates, _fixedUpdates, _lateUpdates);
            }

            _updateCallbacksContainer.OnUpdate += OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate += OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate += OnLateUpdate;
        }

        private async UniTask InitializeSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken, bool addToUpdateLists)
        {
            _diContainer.Inject(gameSystem);
            await gameSystem.InitializeAsync(cancellationToken);
            if (addToUpdateLists)
            {
                gameSystem.AddToUpdateLists(_updates, _fixedUpdates, _lateUpdates);
            }
            _activeGameSystems.Add(gameSystem);
        }
        
        private async UniTask ReleaseSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            gameSystem.RemoveFromUpdateLists(_updates, _fixedUpdates, _lateUpdates);
            _activeGameSystems.Remove(gameSystem);
            await gameSystem.ReleaseAsync(cancellationToken);
        }

        protected override async UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var system in _activeGameSystems)
            {
                await ReleaseSystemAsync(system, cancellationToken);
            }

            _updates.Clear();
            _fixedUpdates.Clear();
            _lateUpdates.Clear();
            _activeGameSystems.Clear();

            _updateCallbacksContainer.OnUpdate -= OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate -= OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate -= OnLateUpdate;
        }

        public void OnUpdate()
        {
            foreach (var updatable in _updates)
            {
                updatable.OnUpdate();
            }
        }

        public void OnFixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedUpdates)
            {
                fixedUpdatable.OnFixedUpdate();
            }
        }

        public void OnLateUpdate()
        {
            foreach (var lateUpdatable in _lateUpdates)
            {
                lateUpdatable.OnLateUpdate();
            }
        }
    }
}
