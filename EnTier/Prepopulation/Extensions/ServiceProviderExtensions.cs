using System;
using System.Reflection;

namespace EnTier.Prepopulation.Extensions
{
    public static class ServiceProviderExtensions
    {



        public static IServiceProvider PerformPrepopulation(this IServiceProvider provider,Assembly assembly)
        {
            var resolver = new DotnetResolver(provider);

            PrepopulationManager.GetInstance().PerformPrepopulation(resolver,assembly);

            return provider;
        }
    }
}