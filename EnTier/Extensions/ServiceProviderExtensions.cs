using System;
using EnTier.Fixture;
using EnTier.Logging;
using Microsoft.Extensions.Logging;

namespace EnTier.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider UseFixture<TFixture>(this IServiceProvider serviceProvider)
        {
            var serviceResolver = new ServiceProviderFixtureResolver(serviceProvider);

            var executer = new FixtureExecuter(serviceResolver);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                EnTierLogging.GetInstance().Logger.LogError(e,"Problem executing Fixture.");
            }

            return serviceProvider;
        }
    }
}