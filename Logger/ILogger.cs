using System;

namespace Whaledevelop
{
    public interface ILogger
    {
        void Flush();
        void Info(object log);
        void Warning(object log);
        void Error(object log);
        void Exception(Exception exception);
    }
}