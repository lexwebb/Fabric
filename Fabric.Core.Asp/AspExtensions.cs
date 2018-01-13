using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Fabric.Core.Asp
{
    public static class AspExtensions
    {
        public static void AddFabric(this IServiceCollection serviceCollection, Action<FabricOptions> setupOptions = null) {
            var options = new FabricOptions();

            if (setupOptions == null) {
                options = FabricOptions.Deafult();
            }
            else {
                setupOptions.Invoke(options);
            }

            serviceCollection.AddSingleton(typeof(IFabricStore), new FabricStore(options));
        }
    }
}
