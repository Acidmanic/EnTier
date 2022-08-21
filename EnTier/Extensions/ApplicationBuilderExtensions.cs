using System;
using System.Data.SqlTypes;
using EnTier;
using EnTier.Fixture;
using EnTier.Logging;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFixture<TFixture>(this IApplicationBuilder app)
        {
            var serviceResolver = new ServiceProviderFixtureResolver(app.ApplicationServices);

            FixtureManager.UseFixture<TFixture>(serviceResolver);

            return app;
        }

        public static IApplicationBuilder UseRepository<TStorage, TId, TRepository>(this IApplicationBuilder app)
            where TRepository : ICrudRepository<TStorage, TId> where TStorage : class, new()
        {
            UnitOfWorkRepositoryConfigurations.GetInstance().RegisterCustomRepository<TStorage, TId, TRepository>();

            return app;
        }

        public static IApplicationBuilder UseLoggerForEnTier(this IApplicationBuilder app, ILogger logger)
        {
            EnTierLogging.GetInstance().Set(logger);

            return app;
        }

        public static IApplicationBuilder UseRegisteredLoggerForEnTier(this IApplicationBuilder app)
        {
            ILogger logger = NullLogger.Instance;

            try
            {
                logger = (ILogger) app.ApplicationServices.GetService(typeof(ILogger));
            }
            catch (Exception e)
            {
            }
            
            EnTierLogging.GetInstance().Set(logger);

            return app;
        }
    }
}