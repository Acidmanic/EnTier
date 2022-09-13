using EnTier.Extensions;
using Meadow;
using Meadow.Extensions;
using Meadow.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Meadow
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
            services.AddControllers();

            new ConsoleLogger().Disable(LogLevel.Debug).UseLoggerForEnTier().UseForMeadow();
            
            // You could call this method in a simpler way, by just passing a MeadowConfiguration object.
            // like: services.AddMeadowUnitOfWork(new MeadowConfiguration
            // {
            //    ConnectionString = "..",
            //    BuildupScriptDirectory ="Scripts"
            // })
            services.AddMeadowUnitOfWork(new MeadowConfigurationProvider());
            
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.IntroduceDotnetResolverToEnTier();
            
            var engine = new MeadowEngine(new MeadowConfigurationProvider().GetConfigurations());

            engine.UseSqlServer();
            
            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            
            engine.CreateDatabase();
            
            engine.BuildUpDatabase();
        }
    }
}
