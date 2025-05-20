using System;
using System.Collections.Generic;

namespace Whaledevelop
{
    public static class DisposableExtensions
    {
        public static void Dispose(this List<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
            {
                disposable?.Dispose();
            }

            disposables.Clear();
        }
    }
}