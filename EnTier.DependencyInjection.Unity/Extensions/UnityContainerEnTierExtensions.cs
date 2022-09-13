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

        public static IUnityContainer IntroduceUnityDiToEnTier(this IUnityContainer container)
        {
            
            EnTierEssence.IntroduceDiResolver(new UnityResolverFacade(container));
            
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