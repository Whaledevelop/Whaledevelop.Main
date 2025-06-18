using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop.GameStates
{
    public interface IGameStatesService<in T> : IService where T : Enum
    {
        UniTask ChangeStateAsync(T stateCode, CancellationToken cancellationToken);
    }
}