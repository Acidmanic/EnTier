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
        public static IServiceCollection AddEntityFrameworkUnitOfWork<TDbContext>(this IServiceCollection services)
        where TDbContext :DbContext
        {
            if (!services.IsRegistered<DbContext>())
            {
                services.AddTransient<DbContext,TDbContext>();
                services.AddTransient<IUnitOfWork,EntityFrameworkUnitOfWork>();
            }

            return services;
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
            if (!services.IsRegistered<DbContext>())
            {
                services.AddTransient<DbContext>(sp => contextFactory());
                
                services.AddTransient<IUnitOfWork,EntityFrameworkUnitOfWork>();
            }

            return services;
        }
    }
}