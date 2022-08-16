using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meadow;
using Meadow.Configuration;
using Meadow.Log;
using Meadow.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            
            var engine = new MeadowEngine(new MeadowConfigurationProvider().GetConfigurations(),new ConsoleLogger());

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
