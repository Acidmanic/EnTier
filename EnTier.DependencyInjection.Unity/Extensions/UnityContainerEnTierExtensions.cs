using System;
using EnTier;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.DependencyInjection.Unity;
using EnTier.UnitOfWork;

// ReSharper disable once CheckNamespace
namespace Unity
{
    public static class UnityContainerEnTierExtensions
    {
        
        internal static EnTierEssence GetEssence(this IUnityContainer container)
        {
            
            var essence = container.Resolve(typeof(EnTierEssence)) as EnTierEssence;

            if (essence == null)
            {
                throw new Exception("You Should register EnTier Essence Object before being able to configure it.");
            }

            return essence;
        }

        public static IUnityContainer IntroduceUnityDiToEnTier(this IUnityContainer container)
        {
            var essence = container.GetEssence();

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
    }
}