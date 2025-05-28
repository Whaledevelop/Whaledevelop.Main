using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.Transitions
{
    public interface IGameTransition
    {
        UniTask BeginAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken);

        UniTask EndAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken);
    }
}