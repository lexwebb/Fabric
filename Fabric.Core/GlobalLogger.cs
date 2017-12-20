using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Core
{
    public interface IGlobalLogger {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception exception = null);
    }
}
