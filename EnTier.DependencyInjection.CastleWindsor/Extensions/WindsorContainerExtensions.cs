using System;
using System.Diagnostics;
using System.Reflection;
using Castle.MicroKernel.Registration;
using EnTier;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.DependencyInjection.CastleWindsor;
using EnTier.EventSourcing;
using EnTier.Fixture;
using EnTier.UnitOfWork;
using EnTier.Utility.MultiplexingStreamEventPublisher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// ReSharper disable once CheckNamespace
namespace Castle.Windsor
{
    public static class WindsorContainerExtensions
    {

        public static EnTierEssence AddEnTier(this IWindsorContainer container, params Assembly[] additionalAssemblies)
        {

            var essence = new EnTierEssence(additionalAssemblies);

            container.Register(Component.For<EnTierEssence>().Instance(essence).LifestyleSingleton());
            
            return essence;
        }

        public static bool IsRegistered<TService>(this IWindsorContainer container)
        {
            var type = typeof(TService);

            return container.Kernel.HasComponent(type);
        }
        
        public static IWindsorContainer ConfigureEnTierResolver(this IWindsorContainer container)
        {

            var essence = GetRegisteredEnTierEssence(container);
            
            essence.UseResolver(new CastleWindsorResolverFacade(container));
            
            return container;
        }

        public static TService GetService<TService>(this IWindsorContainer container)
        {
            var type = typeof(TService);
            
            var serviceObject = container.Resolve(type);

            if (serviceObject is TService service)
            {
                return service;
            }

            return default;
        }
        public static EnTierEssence GetRegisteredEnTierEssence(this IWindsorContainer container)
        {
            var essence = container.GetService<EnTierEssence>();

            if (essence == null)
            {
                throw new Exception(
                    "You have to register EnTierEssence instance before configuring EnTier's Resolver.");
            }

            return essence;
        }
        
        public static IWindsorContainer AddJsonFileUnitOfWork(this IWindsorContainer container)
        {
            return container.Register(Component.For<IUnitOfWork,JsonFileUnitOfWork>());
        }
        
        public static IWindsorContainer AddInMemoryUnitOfWork(this IWindsorContainer container)
        {
            return container.Register(Component.For<IUnitOfWork,InMemoryUnitOfWork>());
        }
        
        public static IWindsorContainer UseFixture<TFixture>(this IWindsorContainer container)
        {
            var essence = GetRegisteredEnTierEssence(container);
            
            var executer = new FixtureExecuter(essence);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                essence.Logger.LogError(e,"Error occured during execution of fixture: {FixtureTypeFullName}",typeof(TFixture).FullName);
            }

            return container;
        }
        
        
        public static IWindsorContainer AddMultiplexingStreamEventPublisher(this IWindsorContainer container)
        {
            
            container.Register(
                Component.For<IStreamEventPublisherAdapter,MultiplexingStreamEventPublisher>()
                .LifestyleSingleton());

            return container;
        }
        
        public static IWindsorContainer ConfigureMultiplexingStreamEventPublishers(this IWindsorContainer container,
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