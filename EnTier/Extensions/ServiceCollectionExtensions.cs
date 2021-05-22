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
    }
}