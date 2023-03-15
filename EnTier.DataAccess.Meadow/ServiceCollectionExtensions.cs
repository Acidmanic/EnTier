using EnTier.DataAccess.Meadow;
using EnTier.UnitOfWork;
using Meadow.Configuration;
using Meadow.Contracts;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMeadowUnitOfWork(this IServiceCollection services,
            MeadowConfiguration configuration)
        {

            var configurationProvider = new ByInstanceMeadowConfigurationProvider(configuration);

            services.AddSingleton<IMeadowConfigurationProvider>(configurationProvider);
            
            services.AddSingleton<IUnitOfWork,MeadowUnitOfWork>();

            return services;
        }


        public static IServiceCollection AddMeadowUnitOfWork
            (this IServiceCollection services,IMeadowConfigurationProvider configurationProvider)
        {
            services.AddSingleton<IMeadowConfigurationProvider>(configurationProvider);
            
            services.AddSingleton<IUnitOfWork,MeadowUnitOfWork>();

            return services;
        }

        public static IServiceCollection AddMeadowUnitOfWork<TConfigurationProvider>(this IServiceCollection services)
        where TConfigurationProvider: class, IMeadowConfigurationProvider
        {
            services.AddSingleton<IMeadowConfigurationProvider, TConfigurationProvider>();
            
            services.AddSingleton<IUnitOfWork,MeadowUnitOfWork>();

            return services;
        }
    }
}