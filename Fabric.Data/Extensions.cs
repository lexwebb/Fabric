using System;
using Unity;

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

        /// <summary>
        /// Registers the instance or default if instance is null.
        /// </summary>
        /// <typeparam name="T">The type of the interface to register</typeparam>
        /// <typeparam name="TD">The type of the default to register.</typeparam>
        /// <param name="unityContainer">The unity container.</param>
        /// <param name="instance">The instance to register</param>
        /// <returns></returns>
        public static IUnityContainer RegisterInstanceOrDefault<T, TD>(this IUnityContainer unityContainer, T instance)
            where TD : T {
            if (instance != null) {
                unityContainer.RegisterInstance(instance);
            }
            else {
                unityContainer.RegisterSingleton<T, TD>();
            }

            return unityContainer;
        }
    }
}
