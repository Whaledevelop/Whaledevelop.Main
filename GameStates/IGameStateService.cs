using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    public interface IGameStateService : IService
    {
        IGameState CurrentState { get; }
        
        UniTask ChangeStateAsync(IGameState state, CancellationToken cancellationToken);
    }
}