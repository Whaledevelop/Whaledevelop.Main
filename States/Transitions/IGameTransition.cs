using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.GameStates
{
    public interface IGameTransition
    {
        UniTask BeginAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken);

        UniTask EndAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken);
    }
}