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

namespace Whaledevelop
{
    [CreateAssetMenu(menuName = "Whaledevelop/Services/GameSystemsService", fileName = "GameSystemsService")]
    public class GameSystemsService : Service, IGameSystemsService, IUpdate, IFixedUpdate, ILateUpdate
    {
        [SerializeField] private GameSystem[] _initialGameSystems;
        
        [Inject]
        private IDiContainer _diContainer;

        [Inject] 
        private IUpdateCallbacksContainer _updateCallbacksContainer;
        
        private readonly List<IUpdate> _updates = new();
        private readonly List<IFixedUpdate> _fixedUpdates = new();
        private readonly List<ILateUpdate> _lateUpdates = new();
        private readonly List<IGameSystem> _activeGameSystems = new();


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
                system.RemoveFromUpdateLists(_updates, _fixedUpdates, _lateUpdates);
                await system.ReleaseAsync(cancellationToken);
                _activeGameSystems.Remove(system);
            }
            var systemsToAdd = gameSystems.Except(_activeGameSystems);
            foreach (var system in systemsToAdd)
            {
                await InitializeSystemAsync(system, cancellationToken);
            }
        }
        
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            foreach (var system in _initialGameSystems)
            {
                await InitializeSystemAsync(system, cancellationToken);
            }
            _updateCallbacksContainer.OnUpdate += OnUpdate;
            _updateCallbacksContainer.OnFixedUpdate += OnFixedUpdate;
            _updateCallbacksContainer.OnLateUpdate += OnLateUpdate;
        }

        private async UniTask InitializeSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken)
        {
            _diContainer.Inject(gameSystem);
            await gameSystem.InitializeAsync(cancellationToken);
            gameSystem.AddToUpdateLists(_updates, _fixedUpdates, _lateUpdates);
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
        
#if UNITY_EDITOR
        
        // TODO сделать окно с выбором папки
        private const string SearchPath = "Assets/_Game/Scriptables/Systems";
        
        [Button("Find Systems in Folder")]
        private void FindAllSystemsInFolder()
        {
            var guids = AssetDatabase.FindAssets("t:GameSystemScriptable", new[] { SearchPath });
            var found = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameSystem>)
                .Where(x => x != null)
                .ToList();

            var current = new HashSet<GameSystem>(_initialGameSystems);
            current.UnionWith(found);

            _initialGameSystems = current.ToArray();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
