using System;
using EnTier;
using EnTier.DependencyInjection;
using EnTier.Exceptions;
using EnTier.Extensions;
using EnTier.Fixture;
using EnTier.Utility.MultiplexingStreamEventPublisher;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFixture<TFixture>(this IApplicationBuilder app)
        {
            if (app.ApplicationServices.GetService(typeof(EnTierEssence)) is EnTierEssence essence)
            {
                FixtureManager.UseFixture<TFixture>(essence);    
            }
            
            return app;
        }
        
        public static T GetService<T>(this IApplicationBuilder app) where T : class
        {
            return app.ApplicationServices.GetService(typeof(T)) as T;
        }

        public static EnTierEssence GetRegisteredEnTierEssence(this IApplicationBuilder app)
        {
            var essence = app.GetService<EnTierEssence>();

            if (essence == null)
            {
                throw new EnTierEssenceIsNotRegisteredException();
            }

            return essence;
        }

        public static IApplicationBuilder ConfigureEnTierResolver(this IApplicationBuilder app)
        {
            var essence = app.GetRegisteredEnTierEssence();

            essence.UseResolver(new DotnetResolverFacade(app.ApplicationServices));

            return app;
        }
        
        public static IApplicationBuilder ConfigureMultiplexingStreamEventPublishers(this IApplicationBuilder app,
            Action<IMultiplexingStreamEventPublisherConfigurations> configurationExpression)
        {

            app.ApplicationServices.ConfigureMultiplexingStreamEventPublishers(configurationExpression);

            return app;
        }
    }
}