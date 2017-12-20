using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Fabric.Core {
    public class ConsoleGlobalLogger : IGlobalLogger {
        public void Info(string message) {
            Debug.WriteLine($"{DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}|INFO|{message}");
        }

        public void Warning(string message) {
            Debug.WriteLine($"{DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}|WARNING|{message}");
        }

        public void Error(string message, Exception exception = null) {
            Debug.WriteLine($"{DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}|ERROR|{message}");
            if (exception != null) {
                Debug.WriteLine(
                    $"{DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}|ERROR|{exception.Message}");
            }
        }
    }
}
