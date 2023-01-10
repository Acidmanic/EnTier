using System;
using EnTier.DependencyInjection;
using EnTier.Exceptions;
using EnTier.Fixture;

namespace EnTier.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider UseFixture<TFixture>(this IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(EnTierEssence)) is EnTierEssence essence)
            {
                FixtureManager.UseFixture<TFixture>(essence);    
            }
            
            return serviceProvider;
        }
        
        public static EnTierEssence GetRegisteredEnTierEssence(this IServiceProvider serviceProvider)
        {
            var essence = serviceProvider.GetService(typeof(EnTierEssence)) as EnTierEssence;

            if (essence == null)
            {
                throw new EnTierEssenceIsNotRegisteredException();
            }

            return essence;
        }
        
        public static IServiceProvider ConfigureEnTierResolver(this IServiceProvider serviceProvider)
        {
            var essence = serviceProvider.GetRegisteredEnTierEssence();

            essence.UseResolver(new DotnetResolverFacade(serviceProvider));

            return serviceProvider;
        }

        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            var type = typeof(T);

            var serviceObject = serviceProvider.GetService(type);

            if (serviceObject is T service)
            {
                return service;
            }

            return default;
        }
    }
}