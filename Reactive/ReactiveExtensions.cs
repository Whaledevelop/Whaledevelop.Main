using System;
using System.Collections.Generic;

namespace Whaledevelop.Reactive
{
    public static class ReactiveExtensions
    {
        public static IDisposable AddToCollection(this IDisposable disposable, ICollection<IDisposable> disposables)
        {
            if (disposables == null)
            {
                throw new ArgumentNullException(nameof(disposables));
            }

            disposables.Add(disposable);

            return disposable;
        }
    }
}