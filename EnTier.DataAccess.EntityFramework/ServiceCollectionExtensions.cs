using EnTier.DataAccess.EntityFramework;
using EnTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkUnitOfWork(this IServiceCollection services,
            DbContext context)
        {
            return services.AddSingleton<IUnitOfWork>(new EntityFrameworkUnitOfWork(context));
        }
    }
}