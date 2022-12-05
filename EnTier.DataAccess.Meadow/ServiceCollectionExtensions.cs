using System;
using System.Runtime.CompilerServices;
using EnTier.DataAccess.Meadow;
using EnTier.UnitOfWork;
using Litbid.DataAccess.Meadow;
using Litbid.DataAccess.Meadow.EnTier.DataAccess.Meadow;
using Meadow.Configuration;
using Meadow.Scaffolding.Contracts;

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