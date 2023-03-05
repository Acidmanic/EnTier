using System;
using EnTier.DependencyInjection;
using EnTier.Exceptions;
using EnTier.Fixture;
using EnTier.Utility.MultiplexingStreamEventPublisher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

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

        public static IServiceProvider ConfigureMultiplexingStreamEventPublishers(this IServiceProvider serviceProvider,
            Action<IMultiplexingStreamEventPublisherConfigurations> configurationExpression)
        {

            var publisher = serviceProvider.GetService<IMultiplexingStreamEventPublisherConfigurations>();

            if (publisher is MultiplexingStreamEventPublisher multiplexingStreamEventPublisher)
            {
                configurationExpression(
                    new MultiplexingStreamEventPublisherConfigurations(multiplexingStreamEventPublisher));
            }
            else
            {
                var logger = serviceProvider.GetService<ILogger>() ?? new ConsoleLogger();
                
                logger.LogError("In order to configure and use MultiplexingStreamEventPublisher in your project," +
                                "you have to first register it in your di-container.");
            }

            return serviceProvider;
        }
    }
}