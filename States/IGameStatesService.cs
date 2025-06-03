using System.Threading;
using Cysharp.Threading.Tasks;
using Sopka;
using Whaledevelop.Reactive;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    public interface IGameStatesService : IService
    {
        ReactiveValue<IGameState> CurrentState { get; }
        
        UniTask ChangeStateAsync(IGameState nextState, CancellationToken cancellationToken);
    }
}