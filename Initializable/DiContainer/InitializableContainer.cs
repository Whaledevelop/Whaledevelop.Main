using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public abstract class InitializableContainer<T> : IInitializable where T : class, IInitializable
    {
        private readonly T[] _items;

        public bool Initialized { get; private set; }
        
        private IDiContainerWrapper _diContainerWrapper;
        
        public void Construct(IDiContainerWrapper diContainerWrapper)
        {
            _diContainerWrapper = diContainerWrapper;
        }
        
        protected InitializableContainer(T[] items)
        {
            _items = items;
        }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            if (Initialized)
            {
                Debug.Log($"Container of {typeof(T)} already initialized");
                return;
            }
            foreach (var item in _items)
            {
                if (item.Initialized)
                {
                    continue;
                }
                _diContainerWrapper.Inject(item);
                await item.InitializeAsync(cancellationToken);
            }
            Initialized = true;
        }
        
        public async UniTask ReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var initializable in _items)
            {
                await initializable.ReleaseAsync(cancellationToken);
            }
            Initialized = false;
        }
    }
}