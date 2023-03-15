using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Reflection.Extensions;
using EnTier;
using EnTier.DataAccess.JsonFile;
using EnTier.EventStore.WebView;
using Example.WebView.DoubleAggregate.SeedGrowing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example.WebView.DoubleAggregate
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
            services.AddControllers().AddApplicationPart(typeof(EventStoreController).Assembly);

            services.AddEnTier();

            services.AddJsonFileUnitOfWork();
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

            GetType().Assembly.ScanForAggregates();

            app.ConfigureEnTierResolver();
            
            new JsonFileUnitOfWork(app.GetService<EnTierEssence>()).ClearAllData();
            
            new HumanSeedGrowing(app.ApplicationServices).GrowHumans(5, 15).Wait();
            
            new CatSeedGrowing(app.ApplicationServices).GrowHumans(5, 15).Wait();
        }
    }
}
