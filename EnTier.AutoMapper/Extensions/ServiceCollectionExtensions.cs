using System;
using AutoMapper;
using EnTier.AutoMapper;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapperForEnTier(this IServiceCollection services,
            IConfigurationProvider configurationProvider)
        {
            services.AddSingleton<EnTier.Mapper.IMapper>(new AutoMapperAdapter(configurationProvider));

            return services;
        }
        public static IServiceCollection AddAutoMapperForEnTier(this IServiceCollection services,
            Action<IMapperConfigurationExpression> configure)
        {
            services.AddSingleton<EnTier.Mapper.IMapper>(new AutoMapperAdapter(
                new MapperConfiguration(configure)
                ));

            return services;
        }
    }
}