using Castle.Facilities.AspNetCore;
using Castle.Windsor;
using Example.CastleWindsor.Contracts;
using Example.CastleWindsor.Controllers;
using Example.CastleWindsor.Fixtures;
using Example.CastleWindsor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Component = Castle.MicroKernel.Registration.Component;

namespace Example.CastleWindsor
{
    public class Startup
    {
        private static readonly WindsorContainer Container = new WindsorContainer();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Windsor setups...
            Container.AddFacility<AspNetCoreFacility>(f => f.CrossWiresInto(services));
            services.AddMvc();
            Container.Register(Component.For<IHttpContextAccessor>().ImplementedBy<HttpContextAccessor>());
            services.AddWindsor(Container,
                opts => opts.UseEntryAssembly(typeof(PostsController).Assembly), // <- Recommended
                () => services.BuildServiceProvider(validateScopes: false)); // <- Optional
            // Introducing CastleWindsor to EnTier (needed for EnTier to work properly)

            Container.AddEnTier();

            Container.ConfigureEnTierResolver();

            // Adding a dependency
            Container.Register(Component.For<ITitleSuggestionService>().ImplementedBy<TitleSuggestionService>());
            // Running a fixture for prepopulating the db
            Container.UseFixture<WelcomePostFixture>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Windsor setups...
            Container.GetFacility<AspNetCoreFacility>().RegistersMiddlewareInto(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(builder => { builder.MapControllers(); });
        }
    }
}