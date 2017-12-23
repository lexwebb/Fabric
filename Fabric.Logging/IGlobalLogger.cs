using System;

namespace Fabric.Logging
{
    public interface IGlobalLogger {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception exception = null);
    }
}
