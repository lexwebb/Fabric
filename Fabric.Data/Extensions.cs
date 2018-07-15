using System;
using Unity;

namespace Fabric.Data {
    public static class Extensions {
        /// <summary>
        ///     Gets the timestamp.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetTimestamp(this DateTime value) {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        public static FabricDatabase UseCouchbase(this FabricDatabase database) {
            database.Resolver.RegisterSingleton<IDatabaseHelper, CouchbaseDatabaseHelper>();

            return database;
        }
    }
}
