using EnTier;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.UnitOfWork;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static bool IsRegistered<TService>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            
            for (int i = 0; i < services.Count; i++)
            {
                if (services[i].ServiceType == serviceType)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static EnTierEssence AddEnTier(this IServiceCollection services)
        {
            var essence = new EnTierEssence();

            services.AddSingleton<EnTierEssence>(essence);

            return essence;
        }
        
        public static IServiceCollection AddJsonFileUnitOfWork(this IServiceCollection services)
        {
            if (!services.IsRegistered<EnTierEssence>())
            {
                services.AddEnTier();
            }
            return services.AddSingleton<IUnitOfWork,JsonFileUnitOfWork>();
        }
        
        public static IServiceCollection AddInMemoryUnitOfWork(this IServiceCollection services)
        {
            if (!services.IsRegistered<EnTierEssence>())
            {
                services.AddEnTier();
            }
            return services.AddSingleton<IUnitOfWork,InMemoryUnitOfWork>();
        }
    }
}