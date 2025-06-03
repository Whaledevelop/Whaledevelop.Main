using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.GameSystems;

namespace Whaledevelop.GameStates
{
    public interface IGameState : IInitializable
    {
        UniTask EnableAsync(CancellationToken cancellationToken);
        
        UniTask DisableAsync(CancellationToken cancellationToken);
        
        string Name { get; }
        IEnumerable<IGameSystem> RequiredGameSystems { get; }
    }
}