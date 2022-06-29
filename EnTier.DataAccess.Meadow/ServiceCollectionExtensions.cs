using System;
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
            return services.AddSingleton<IUnitOfWork>(new MeadowUnitOfWork(configuration));
        }


        public static IServiceCollection AddMeadowUnitOfWork(this IServiceCollection services,
            IMeadowConfigurationProvider configurationProvider)
        {
            return services.AddSingleton<IUnitOfWork>(new MeadowUnitOfWork(configurationProvider));
        }
    }
}