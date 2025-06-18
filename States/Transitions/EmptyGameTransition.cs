using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.GameStates
{
    [Serializable]
    public class EmptyGameTransition : IGameTransition
    {
        public UniTask BeginAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken)
        {
            return  UniTask.CompletedTask;
        }

        public UniTask EndAsync(IGameState fromState, IGameState toState, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}