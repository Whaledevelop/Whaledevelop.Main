using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop
{
    public interface IAsyncAction : IAction
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}