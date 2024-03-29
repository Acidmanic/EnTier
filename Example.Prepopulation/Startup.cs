using EnTier.Services;
using Example.Prepopulation.Models;
using Example.Prepopulation.Prepopulation;
using Example.Prepopulation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.Prepopulation
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

            services.AddEnTier();

            services.AddInMemoryUnitOfWork();
            
            services.AddTransient<PostsSeed>();
            services.AddTransient<UsersSeed>();

            var logger = new ConsoleLogger();

            services.AddTransient<ILogger>(sp => logger);

            services.AddTransient<IUserNameProvider, AdminUserNameProvider>();
            services.AddTransient<ICrudService<Chocolate, long>, ChocolateManipulatedService>();
            
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
            
            app.PerformPrepopulation(typeof(Startup).Assembly);
        }
    }
}
