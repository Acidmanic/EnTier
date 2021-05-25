using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.UnitOfWork;

// ReSharper disable once CheckNamespace
namespace Unity
{
    public static class UnityContainerEnTierExtensions
    {
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