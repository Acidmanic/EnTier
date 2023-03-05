using System;
using System.Reflection;
using System.Security;
using EnTier;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.DependencyInjection.Unity;
using EnTier.EventSourcing;
using EnTier.Fixture;
using EnTier.UnitOfWork;
using EnTier.Utility.MultiplexingStreamEventPublisher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;
using Unity.Lifetime;

// ReSharper disable once CheckNamespace
namespace Unity
{
    public static class UnityContainerEnTierExtensions
    {
        public static bool IsRegistered<TService>(this IUnityContainer container)
        {
            var type = typeof(TService);

            return container.IsRegistered(type);
        }


        public static TService GetService<TService>(this IUnityContainer container)
        {
            var type = typeof(TService);

            var serviceObject = container.Resolve(typeof(EnTierEssence));

            if (serviceObject is TService service)
            {
                return service;
            }

            return default;
        }

        public static EnTierEssence GetRegisteredEnTierEssence(this IUnityContainer container)
        {
            var essence = GetService<EnTierEssence>(container);

            return essence;
        }

        public static IUnityContainer ConfigureEnTierResolver(this IUnityContainer container)
        {
            var essence = container.GetRegisteredEnTierEssence();

            if (essence == null)
            {
                throw new Exception("You Should register EnTier Essence Object before being able to configure it.");
            }

            essence.UseResolver(new UnityResolverFacade(container));

            return container;
        }

        public static IUnityContainer AddJsonFileUnitOfWork(this IUnityContainer container)
        {
            return container.RegisterSingleton<IUnitOfWork, JsonFileUnitOfWork>();
        }

        public static IUnityContainer AddInMemoryUnitOfWork(this IUnityContainer container)
        {
            return container.RegisterSingleton<IUnitOfWork, InMemoryUnitOfWork>();
        }

        public static EnTierEssence AddEnTier(this IUnityContainer container, params Assembly[] additionalAssemblies)
        {
            var essence = new EnTierEssence(additionalAssemblies);

            container.RegisterInstance(essence);

            return essence;
        }

        public static IUnityContainer UseFixture<TFixture>(this IUnityContainer container)
        {
            var essence = container.GetRegisteredEnTierEssence();

            if (essence == null)
            {
                throw new Exception("You Should register EnTier Essence Object before being able to configure it.");
            }

            FixtureManager.UseFixture<TFixture>(essence);

            return container;
        }

        public static IUnityContainer AddMultiplexingStreamEventPublisher(this IUnityContainer container)
        {
            container.RegisterType<IStreamEventPublisherAdapter,MultiplexingStreamEventPublisher>
                (new SingletonLifetimeManager());

            return container;
        }

        public static IUnityContainer ConfigureMultiplexingStreamEventPublishers(this IUnityContainer container,
            Action<IMultiplexingStreamEventPublisherConfigurations> configurationExpression)
        {
            var publisher = container.GetService<IMultiplexingStreamEventPublisherConfigurations>();

            if (publisher is MultiplexingStreamEventPublisher multiplexingStreamEventPublisher)
            {
                configurationExpression(
                    new MultiplexingStreamEventPublisherConfigurations(multiplexingStreamEventPublisher));
            }
            else
            {
                var logger = container.GetService<ILogger>() ?? new ConsoleLogger();

                logger.LogError("In order to configure and use MultiplexingStreamEventPublisher in your project," +
                                "you have to first register it in your di-container.");
            }

            return container;
        }
    }
}