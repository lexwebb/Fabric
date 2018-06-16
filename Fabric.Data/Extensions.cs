using System;

namespace Fabric.Data {
    internal static class Extensions {
        /// <summary>
        ///     Gets the timestamp.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetTimestamp(this DateTime value) {
            return value.ToString("yyyyMMddHHmmssfff");
        }
    }
}