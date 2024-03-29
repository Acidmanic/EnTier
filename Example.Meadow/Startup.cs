using System;
using Example.Meadow.Configurations;
using Meadow;
using Meadow.Contracts;
using Meadow.MySql;
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

        private static readonly IMeadowConfigurationProvider MeadowConfigurationProvider = new MySqlConfigurationProvider();

        private static readonly Action<MeadowEngine> SetMeadowDatabase = e => e.UseMySql();
        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEnTier();
            
            services.AddTransient<ILogger>(sp => new ConsoleLogger().Disable(LogLevel.Debug));
            
            services.AddMeadowUnitOfWork(MeadowConfigurationProvider);
            
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

            app.ConfigureEnTierResolver();
            
            var engine = new MeadowEngine(MeadowConfigurationProvider.GetConfigurations());
            
            MeadowEngine.UseLogger(app.ApplicationServices.GetService(typeof(ILogger)) as ILogger);

            SetMeadowDatabase(engine);
            
            if (engine.DatabaseExists())
            {
                engine.DropDatabase();
            }
            
            engine.CreateDatabase();
            
            engine.BuildUpDatabase();


            app.UseFixture<PostsFixture>();
        }
    }
}
