using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Whaledevelop.Extensions;

namespace Whaledevelop.Services
{
    public static class InitializableExtensions
    {
        public static UniTask InitializeAsync(this IEnumerable<IInitializable> self, CancellationToken cancellationToken)
        {
            var tasks = self.Select(initializable => initializable.InitializeAsync(cancellationToken));
            return UniTask.WhenAll(tasks);
        }

        public static UniTask ReleaseAsync(this IEnumerable<IInitializable> self, CancellationToken cancellationToken)
        {
            var tasks = self.Select(initializable => initializable.ReleaseAsync(cancellationToken));
            return UniTask.WhenAll(tasks);
        }
    }
}