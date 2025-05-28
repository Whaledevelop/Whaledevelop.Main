using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop
{
    public abstract class AsyncAction : IAsyncAction
    {
        public virtual void Execute()
        {
            ExecuteAsync().Forget();
        }

        public abstract UniTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}