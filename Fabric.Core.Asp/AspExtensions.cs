using System;
using Microsoft.Extensions.DependencyInjection;

namespace Fabric.Core.Asp
{
    public static class AspExtensions
    {
        public static void AddFabric(this IServiceCollection serviceCollection, Action<FabricOptions> setupOptions) {
            var options = new FabricOptions();
            setupOptions.Invoke(options);
            serviceCollection.AddSingleton(typeof(IFabricStore), new FabricStore(options));
        }

        public static void AddFabric(this IServiceCollection serviceCollection) {
            var options = FabricOptions.Deafult();
            serviceCollection.AddSingleton(typeof(IFabricStore), new FabricStore(options));
        }
    }
}
