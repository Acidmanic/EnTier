using System;
using EnTier.Fixture;
using EnTier.Repositories;
using EnTier.UnitOfWork;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFixture<TFixture>(this IApplicationBuilder app)
        {
            var serviceResolver = new ServiceProviderFixtureResolver(app.ApplicationServices);
            
            var executer = new FixtureExecuter(serviceResolver);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return app;
        }

        public static IApplicationBuilder UseRepository<TAbstraction, TRepository>(this IApplicationBuilder app)
        where TRepository:TAbstraction
        {

            UnitOfWorkRepositoryConfigurations.GetInstance().RegisterCustomRepository<TAbstraction,TRepository>();
            
            return app;
        }
    }
}