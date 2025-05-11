using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public abstract class ScriptableInitializable : ScriptableObject, IInitializable
    {
        [NonSerialized]
        private CancellationTokenSource _cancellationTokenSource;
        
        [NonSerialized]
        private bool _initialized;

        bool IInitializable.Initialized => _initialized;

        async UniTask IInitializable.InitializeAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);

            await OnInitializeAsync(cancellationToken);

            _initialized = true;
        }

        async UniTask IInitializable.ReleaseAsync(CancellationToken cancellationToken)
        {
            await OnReleaseAsync(cancellationToken);

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            
            _initialized = false;
        }

        protected abstract UniTask OnInitializeAsync(CancellationToken cancellationToken);

        protected virtual UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}