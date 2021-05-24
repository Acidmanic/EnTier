using System;
using EnTier.DependencyInjection.CastleWindsor;
using EnTier.Fixture;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFixtureByWindsor<TFixture>(this IApplicationBuilder app)
        {
            var serviceResolver = new CastleWindsorFixtureResolver(app.ApplicationServices);
            
            var executer = new FixtureExecuter(serviceResolver);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                //ignore
                //TODO: Add Logs
            }

            return app;
        }
    }
}