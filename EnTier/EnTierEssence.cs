using System;
using EnTier.DependencyInjection;
using EnTier.Logging;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace EnTier
{
    public static class EnTierEssence
    {


        public static void RegisterCustomRepository<TStorage, TId, TRepository>() where TStorage : class, new()
        {
            UnitOfWorkRepositoryConfigurations.GetInstance().RegisterCustomRepository<TStorage,TId,TRepository>();
        }

        public static Type GetRepositoryType<TStorage, TId>() where TStorage : class, new()
        {
            return UnitOfWorkRepositoryConfigurations.GetInstance().GetRepositoryType<TStorage, TId>();
        }

        public static ILogger Logger => EnTierLogging.GetInstance().Logger;

        public static void SetLogger(ILogger logger)
        {
            EnTierLogging.GetInstance().Set(logger);
        }

        internal static EnTierResolver Resolver { get; private set; } = new NullEnTierResolver(new ConsoleLogger());
      
        public static void IntroduceDiResolver(IResolverFacade resolver)
        {
            Resolver = new EnTierResolver(resolver);
        }

    }
}