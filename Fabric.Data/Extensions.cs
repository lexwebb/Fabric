using System;

namespace Fabric.Data {
    internal static class Extensions {
        public static string GetTimestamp(this DateTime value) {
            return value.ToString("yyyyMMddHHmmssfff");
        }
    }
}
