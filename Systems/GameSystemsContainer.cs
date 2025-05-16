using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public class GameSystemsContainer : Initializable, IUpdatable, IFixedUpdatable, ILateUpdatable
    {
        [Inject]
        private IDiContainer _diContainer;

        [Inject] 
        private IUpdateCallbacksContainer _updateCallbacksContainer;

        private GameSystemsConfig _config;
        
        private List<IUpdatable> _updatables = new();
        private List<IFixedUpdatable> _fixedUpdatables = new();
        private List<ILateUpdatable> _lateUpdatables = new();
        
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

            _updatables.AddRange(_config.GetSorted<IUpdatable>());
            _fixedUpdatables.AddRange(_config.GetSorted<IFixedUpdatable>());
            _lateUpdatables.AddRange(_config.GetSorted<ILateUpdatable>());

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

            _updatables.Clear();
            _fixedUpdatables.Clear();
            _lateUpdatables.Clear();

            _updateCallbacksContainer.OnUpdate -= OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate -= OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate -= OnLateUpdate;
        }

        public void OnUpdate()
        {
            foreach (var updatable in _updatables)
            {
                updatable.OnUpdate();
            }
        }

        public void OnFixedUpdate()
        {
            foreach (var fixedUpdatable in _fixedUpdatables)
            {
                fixedUpdatable.OnFixedUpdate();
            }
        }

        public void OnLateUpdate()
        {
            foreach (var lateUpdatable in _lateUpdatables)
            {
                lateUpdatable.OnLateUpdate();
            }
        }
    }
}
