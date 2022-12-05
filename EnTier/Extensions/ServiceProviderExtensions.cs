using System;
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
    }
}