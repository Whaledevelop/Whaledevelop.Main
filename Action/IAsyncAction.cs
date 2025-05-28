using System.Threading;
using Cysharp.Threading.Tasks;
using Sopka;

namespace Whaledevelop
{
    public interface IAsyncAction : IAction
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}