using System;
using System.Reflection;

namespace EnTier.Prepopulation
{
    public static class ServiceProviderExtensions
    {



        public static IServiceProvider PerformPrepopulation(this IServiceProvider provider,Assembly assembly)
        {
            var resolver = new DotnetResolver(provider);

            PrepopulationManager2.GetInstance().PerformPrepopulation(resolver,assembly);

            return provider;
        }
    }
}