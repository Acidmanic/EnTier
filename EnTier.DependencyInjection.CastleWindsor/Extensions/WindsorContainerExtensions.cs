using System;
using Castle.MicroKernel.Registration;
using EnTier;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.DependencyInjection.CastleWindsor;
using EnTier.Fixture;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Castle.Windsor
{
    public static class WindsorContainerExtensions
    {

        public static IWindsorContainer ConfigureEnTierResolver(this IWindsorContainer container)
        {

            var essence = GetEssence(container);
            
            essence.UseResolver(new CastleWindsorResolverFacade(container));
            
            return container;
        }

        private static EnTierEssence GetEssence(IWindsorContainer container)
        {
            var essence = container.Resolve(typeof(EnTierEssence)) as EnTierEssence;

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
        
        public static IWindsorContainer UseFixtureWithWindsor<TFixture>(this IWindsorContainer container)
        {
            var essence = GetEssence(container);
            
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
    }
}