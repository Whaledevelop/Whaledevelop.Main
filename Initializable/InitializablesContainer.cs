using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public abstract class InitializablesContainer<T> where T : class, IInitializable
    {
        private readonly List<T> _initializables = new();

        private readonly IDiContainer _diContainer;

        private bool _initialized;

        protected InitializablesContainer(IDiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Add(T item)
        {
            if (!_initializables.Contains(item))
            {
                _initializables.Add(item);
            }
        }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
            {
                Debug.Log($"Container of {typeof(T)} already initialized");
                return;
            }
            var initializablesArray = _initializables.ToArray();
            await InitializeAsync(initializablesArray, cancellationToken);
            _initialized = true;
        }
        
        public async UniTask ReleaseAsync(CancellationToken cancellationToken)
        {
            foreach (var initializable in _initializables)
            {
                await initializable.ReleaseAsync(cancellationToken);
            }
            _initializables.Clear();

            _initialized = false;
        }

        private async UniTask InitializeAsync(T[] initializables, CancellationToken cancellationToken)
        {
            if (initializables.All(initializable => initializable.Initialized))
            {
                return;
            }
            foreach (var initializable in initializables)
            {
                _diContainer.Inject(initializable);
            }

            var tasks = Enumerable.Select(initializables, initializable => initializable.InitializeAsync(cancellationToken));
            await UniTask.WhenAll(tasks);
        }
    }
}