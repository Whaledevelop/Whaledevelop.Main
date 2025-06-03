using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Whaledevelop.Services;

namespace Whaledevelop.GameSystems
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameSystemsService", fileName = "GameSystemsService")]
    public class GameSystemsService : Service, IGameSystemsService
    {
        [Inject]
        private IDiContainer _diContainer;

        [Inject] 
        private IUpdateCallbacksContainer _updateCallbacksContainer;
        
        private readonly List<IUpdate> _updates = new();
        private readonly List<IFixedUpdate> _fixedUpdates = new();
        private readonly List<ILateUpdate> _lateUpdates = new();
        private readonly List<IGameSystem> _activeGameSystems = new();

        protected override UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            _updateCallbacksContainer.OnUpdate += OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate += OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate += OnLateUpdate;
            return UniTask.CompletedTask;
        }
        
        protected override async UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            _updates.Clear();
            _fixedUpdates.Clear();
            _lateUpdates.Clear();
            _updateCallbacksContainer.OnUpdate -= OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate -= OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate -= OnLateUpdate;
            
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
            
            _updates.AddIfType(gameSystem);
            _fixedUpdates.AddIfType(gameSystem);
            _lateUpdates.AddIfType(gameSystem);
            
            _activeGameSystems.Add(gameSystem);
        }
        
        private async UniTask ReleaseSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            _updates.RemoveIfType(gameSystem);
            _fixedUpdates.RemoveIfType(gameSystem);
            _lateUpdates.RemoveIfType(gameSystem);
            
            _activeGameSystems.Remove(gameSystem);
            await gameSystem.ReleaseAsync(cancellationToken);
        }
        
        private void OnUpdate()
        {
            foreach (var updatable in _updates)
            {
                updatable.OnUpdate();
            }
        }

        private void OnFixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedUpdates)
            {
                fixedUpdatable.OnFixedUpdate();
            }
        }

        private void OnLateUpdate()
        {
            foreach (var lateUpdatable in _lateUpdates)
            {
                lateUpdatable.OnLateUpdate();
            }
        }
    }
}
