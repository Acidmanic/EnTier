using System;
using EnTier.DataAccess.EntityFramework;
using EnTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a EntityFrameworkUnitOfWork object with given context as a singleton 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityFrameworkUnitOfWork(this IServiceCollection services,
            DbContext context)
        {
            return services.AddSingleton<IUnitOfWork>(new EntityFrameworkUnitOfWork(context));
        }
        
        /// <summary>
        /// Registers The EntityFrameworkUnitOfWork factory as transient (Per Request)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="contextFactory"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityFrameworkUnitOfWork(this IServiceCollection services,
            Func<DbContext> contextFactory)
        {
            return services.AddTransient<IUnitOfWork>(sp => new EntityFrameworkUnitOfWork(contextFactory.Invoke()));
        }
    }
}