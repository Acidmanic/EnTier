using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.Fixture;
using EnTier.UnitOfWork;

namespace EnTier.DependencyInjection.CastleWindsor.Extensions
{
    public static class WindsorContainerExtensions
    {
        public static IWindsorContainer AddJsonFileUnitOfWork(this IWindsorContainer container)
        {
            return container.Register(Component.For<IUnitOfWork,JsonFileUnitOfWork>());
        }
        
        public static IWindsorContainer AddInMemoryUnitOfWork(this IWindsorContainer container)
        {
            return container.Register(Component.For<IUnitOfWork,InMemoryUnitOfWork>());
        }
        
        public static IWindsorContainer UseFixtureByWindsor<TFixture>(this IWindsorContainer container)
        {
            var serviceResolver = new CastleWindsorFixtureResolver(container);
            
            var executer = new FixtureExecuter(serviceResolver);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                //ignore
                //TODO: Add Logs
            }

            return container;
        }
    }
}