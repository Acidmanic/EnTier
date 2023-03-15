using EnTier.EventStore.WebView;
using Meadow;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.EventSourcing.Meadow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().PartManager.ApplicationParts
                .Add(new AssemblyPart(typeof(EventStoreController).Assembly));
            
            services.AddEnTier();
            
            services.AddMeadowUnitOfWork<MeadowConfigurationProvider>();

            services.AddTransient<ILogger>(sp => new ConsoleLogger().Shorten().EnableAll());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.ConfigureEnTierResolver();

            app.ApplicationServices.GetService<ILogger>()?.UseForMeadow();

            var configurations = app.ApplicationServices.GetService<IMeadowConfigurationProvider>()?.GetConfigurations();

            var engine = new MeadowEngine(configurations);

            engine.UseSqlServer();

            // if (engine.DatabaseExists())
            // {
            //     engine.DropDatabase();
            // }

            engine.CreateIfNotExist();

            engine.BuildUpDatabase();


            GetType().Assembly.ScanForAggregates();
        }
    }
}