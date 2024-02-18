using System;
using UnityEngine;

namespace Whaledevelop
{
    public class UnityLogger : ILogger
    {
        public void Flush()
        {
        }

        public void Info(object log)
        {
            Debug.Log(log);
        }

        public void Warning(object log)
        {
            Debug.LogWarning(log);
        }

        public void Error(object log)
        {
            Debug.LogError(log);
        }

        public void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}