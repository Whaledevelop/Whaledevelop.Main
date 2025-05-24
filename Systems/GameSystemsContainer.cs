using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public class GameSystemsContainer : Initializable, IUpdate, IFixedUpdate, ILateUpdate
    {
        [Inject]
        private IDiContainer _diContainer;

        [Inject] 
        private IUpdateCallbacksContainer _updateCallbacksContainer;

        private readonly GameSystemsConfig _config;
        
        private readonly List<IUpdate> _updates = new();
        private readonly List<IFixedUpdate> _fixedUpdates = new();
        private readonly List<ILateUpdate> _lateUpdates = new();
        
        public GameSystemsContainer(GameSystemsConfig config)
        {
            _config = config;
        }
        
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            foreach (var system in _config.GameSystems)
            {
                _diContainer.Inject(system);
                await ((IInitializable)system).InitializeAsync(cancellationToken);
            }

            var systemsHashSet = new HashSet<GameSystemScriptable>();
            foreach (var system in _config.UpdateOrder)
            {
                system.DistributeToUpdateLists(_updates, _fixedUpdates, _lateUpdates);
                systemsHashSet.Add(system);
            }
            foreach (var system in _config.GameSystems)
            {
                if (systemsHashSet.Contains(system))
                {
                    continue;
                }
                system.DistributeToUpdateLists(_updates, _fixedUpdates, _lateUpdates);
            }

            _updateCallbacksContainer.OnUpdate += OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate += OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate += OnLateUpdate;
        }


        protected override async UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var system in _config.GameSystems)
            {
                await ((IInitializable)system).ReleaseAsync(cancellationToken);
            }

            _updates.Clear();
            _fixedUpdates.Clear();
            _lateUpdates.Clear();

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
