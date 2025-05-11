using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop
{
    public interface IInitializable
    {
        bool Initialized { get; }

        UniTask InitializeAsync(CancellationToken cancellationToken);

        UniTask ReleaseAsync(CancellationToken cancellationToken);
    }
}