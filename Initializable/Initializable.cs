using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public abstract class Initializable : IInitializable
    {
        [NonSerialized]
        private CancellationTokenSource _cancellationTokenSource;
        
        [NonSerialized]
        private bool _initialized;

        bool IInitializable.Initialized => _initialized;

        async UniTask IInitializable.InitializeAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
            {
                Debug.Log("Already Initialized");
                return;
            }
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);

            await OnInitializeAsync(cancellationToken);

            _initialized = true;
        }

        async UniTask IInitializable.ReleaseAsync(CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                Debug.Log("Not initialized");
                return;
            }
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            await OnReleaseAsync(cancellationToken);   
            _initialized = false;
        }

        protected virtual UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}