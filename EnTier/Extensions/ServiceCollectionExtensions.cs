using EnTier.DataAccess.InMemory;
using EnTier.DataAccess.JsonFile;
using EnTier.UnitOfWork;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonFileUnitOfWork(this IServiceCollection services)
        {
            return services.AddSingleton<IUnitOfWork>(new JsonFileUnitOfWork());
        }
        
        public static IServiceCollection AddInMemoryUnitOfWork(this IServiceCollection services)
        {
            return services.AddSingleton<IUnitOfWork>(new InMemoryUnitOfWork());
        }
    }
}