using System.Reflection;
using EnTier;
using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.EventSourcing;
using EnTier.UnitOfWork;
using EnTier.Utility;
using EnTier.Utility.MultiplexingStreamEventPublisher;

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

        public static EnTierEssence AddEnTier(this IServiceCollection services, params Assembly[] additionalAssemblies)
        {
            var essence = new EnTierEssence(additionalAssemblies);

            services.AddSingleton<EnTierEssence>(essence);

            return essence;
        }

        public static IServiceCollection AddJsonFileUnitOfWork(this IServiceCollection services)
        {
            if (!services.IsRegistered<EnTierEssence>())
            {
                services.AddEnTier();
            }

            return services.AddSingleton<IUnitOfWork, JsonFileUnitOfWork>();
        }

        public static IServiceCollection AddInMemoryUnitOfWork(this IServiceCollection services)
        {
            if (!services.IsRegistered<EnTierEssence>())
            {
                services.AddEnTier();
            }

            return services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
        }

        public static IServiceCollection AddMultiplexingStreamEventPublisher(this IServiceCollection services)
        {
            services.AddSingleton<IStreamEventPublisherAdapter, MultiplexingStreamEventPublisher>();

            return services;
        }
    }
}