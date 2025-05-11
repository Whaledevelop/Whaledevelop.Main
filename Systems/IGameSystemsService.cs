using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Services;

namespace Whaledevelop
{
    public interface IGameSystemsService : IService
    {
        UniTask UpdateSystemsAsync(IGameSystem[] gameSystems, CancellationToken cancellationToken);
        
        UniTask EnableSystemAsync(IGameSystem gameSystem, CancellationToken cancellationToken);
    }
}