using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop
{
    public interface IGameSystemsService : IService
    {
        UniTask AddSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken);
        UniTask RemoveSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken);
        UniTask UpdateSystemsAsync(IGameSystem[] gameSystems, CancellationToken cancellationToken);
    }
}