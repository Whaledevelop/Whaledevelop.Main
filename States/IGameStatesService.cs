using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    public interface IGameStatesService : IService
    {
        IGameState CurrentState { get; }
        
        UniTask SetStateAsync(IGameState state, CancellationToken cancellationToken);
    }
}