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

        public static IWindsorContainer IntroduceWindsorDiToEnTier(this IWindsorContainer container)
        {
            EnTierEssence.IntroduceDiResolver(new CastleWindsorResolverFacade(container));
            
            return container;
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
            var serviceResolver = new CastleWindsorFixtureResolver(container);
            
            var executer = new FixtureExecuter(serviceResolver);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                EnTierEssence.Logger.LogError(e,"Error occured during execution of fixture: {FixtureTypeFullName}",typeof(TFixture).FullName);
            }

            return container;
        }
    }
}