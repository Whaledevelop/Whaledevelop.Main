using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.Extensions
{
    public static class UniTaskExtensions
    {
        public static UniTask WhenAll(this IEnumerable<UniTask> self)
        {
            return UniTask.WhenAll(self);
        }

        public static UniTask<int> WhenAny(this IEnumerable<UniTask> self)
        {
            return UniTask.WhenAny(self);
        }
    }
}