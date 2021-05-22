using AutoMapper;
using EnTier.AutoMapper;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services,
            IConfigurationProvider configurationProvider)
        {
            services.AddSingleton<EnTier.Mapper.IMapper>(new AutoMapperAdapter(configurationProvider));

            return services;
        }
    }
}