using System;

namespace Fabric.Data
{
    internal static class Extensions
    {
        public static String GetTimestamp(this DateTime value) {
            return value.ToString("yyyyMMddHHmmssfff");
        }
    }
}
