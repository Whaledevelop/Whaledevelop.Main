using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.Services;

namespace Whaledevelop
{
    public abstract class GameState : ScriptableInitializable, IGameState
    {
        [SerializeField]
        private GameSystem[] _gameSystems;

        public virtual UniTask EnableAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask DisableAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public string Name => name;
        
        public IEnumerable<IGameSystem> RequiredGameSystems => _gameSystems;
    }
}