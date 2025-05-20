using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Whaledevelop
{
    public static class UniTaskUtility
    {
        public static void ExecuteAfterSeconds(float seconds, Action callback, CancellationToken cancellationToken = default)
        {
            DelayAndInvoke(seconds, callback, cancellationToken).Forget();
        }

        public static void ExecuteAfterFrames(int frameCount, Action callback, CancellationToken cancellationToken = default)
        {
            DelayFramesAndInvoke(frameCount, callback, cancellationToken).Forget();
        }

        private static async UniTaskVoid DelayAndInvoke(float seconds, Action callback, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: cancellationToken);
                callback?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async UniTaskVoid DelayFramesAndInvoke(int frameCount, Action callback, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.DelayFrame(frameCount, PlayerLoopTiming.Update, cancellationToken);
                callback?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}