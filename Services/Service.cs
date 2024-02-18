using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whaledevelop.Extensions;

namespace Whaledevelop.Services
{
    [Serializable]
    public abstract class Service : IService
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

            _cancellationTokenSource.CancelAndDispose();
            _cancellationTokenSource = null;

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