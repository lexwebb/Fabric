using System;
using System.IO;
using System.Reflection;
using Fabric.Core.Asp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Fabric.Server {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddFabric(options => {
                options.DataFolderName =
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase
                            .Replace(@"file:///", string.Empty)), "FabricDatabase");
            });
            services.AddMvc(options => { options.InputFormatters.Insert(0, new RawJsonBodyInputFormatter()); })
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                });
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes => {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            // this serves my index.html from the wwwroot folder when 
            // a route not containing a file extension is not handled by MVC.  
            // If the route contains a ".", a 404 will be returned instead.
            app.MapWhen(context => context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value),
                branch => {
                    branch.Use((context, next) => {
                        context.Request.Path = new PathString("/home/index");
                        Console.WriteLine("Path changed to:" + context.Request.Path.Value);
                        return next();
                    });

                    branch.UseStaticFiles();
                });
        }
    }
}
