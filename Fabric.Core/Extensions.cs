using System;
using System.IO;

namespace Fabric.Core {
    internal static class Extensions {
        public static string TrimStart(this string target, string trimChars) {
            if (target.StartsWith(trimChars)) {
                var pos = trimChars.Length;
                return target.Substring(pos);
            }
            return target;
        }

        public static string TrimEnd(this string target, string trimChars) {
            if (target.EndsWith(trimChars)) {
                var pos = target.IndexOf(trimChars);
                return target.Substring(0, pos);
            }
            return target;
        }

        /// <summary>
        /// Depth-first recursive delete, with handling for descendant 
        /// directories open in Windows Explorer.
        /// </summary>
        public static void DeleteDirectory(this DirectoryInfo direcotryInfo) {
            foreach (var directory in direcotryInfo.GetDirectories()) {
                DeleteDirectory(directory);
            }

            try {
                direcotryInfo.Delete(true);
            } catch (IOException) {
                direcotryInfo.Delete(true);
            } catch (UnauthorizedAccessException) {
                direcotryInfo.Delete(true);
            }
        }
    }
}
