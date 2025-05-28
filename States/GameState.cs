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

        public abstract UniTask EnableAsync(CancellationToken cancellationToken);

        public abstract UniTask DisableAsync(CancellationToken cancellationToken);

        public string Name => name;
        
        public IEnumerable<IGameSystem> RequiredGameSystems => _gameSystems;
    }
}