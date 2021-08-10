using dotnet_etcd;
using Iolaus.Config.Etcd;
using Iolaus;
using static Iolaus.Nats.NatsRoute;
using Iolaus.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using SpaCliMiddleware;

namespace Atlas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var etcdClient = new EtcdClient("http://localhost:2379");
            var provider = new EtcdConfigProvider(etcdClient);
            var connectionFactory = new ConnectionFactory();
            var connection = connectionFactory.CreateConnection();

            services.AddControllers();
            services.AddSingleton<Iolaus.Config.IConfigurationProvider>(provider);
            services.AddRouteRegistry(registry =>
            {
                registry.Add("NATS", Route);
            });
            services.AddScoped<Router>();
            services.AddSingleton<IConnection>(connection);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();

                // Note: only use spacliproxy in development. 
                // Production should use "UseSpaStaticFiles()"
                endpoints.MapToSpaCliProxy(
                    "{*path}",
                    new SpaOptions { SourcePath = "ClientApp" },
                    npmScript: env.IsDevelopment() ? "autobuild" : "",
                    port: 35729,
                    regex: "Server started",
                    forceKill: true,
                    useProxy: true
                );
            });
        }
    }
}
