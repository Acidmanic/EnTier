using System;
using EnTier.DependencyInjection.Unity.Fixture;
using EnTier.Fixture;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFixtureWithUnity<TFixture>(this IApplicationBuilder app)
        {
            var serviceResolver = new UnityFixtureResolver(app);
            
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
    }
}