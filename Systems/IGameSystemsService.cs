using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop.GameSystems
{
    public interface IGameSystemsService : IService
    {
        UniTask InitializeSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken);
        
        UniTask ReleaseSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken);
    }
}