using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop
{
    public interface IGameStatesService : IService
    {
        UniTask ChangeStateAsync(IGameState nextState, CancellationToken cancellationToken);
    }
}