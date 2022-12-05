using System;
using EnTier;
using EnTier.DependencyInjection.Unity.Fixture;
using EnTier.Fixture;
using Microsoft.Extensions.Logging;
using Unity;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {


        private static EnTierEssence GetEssence(IApplicationBuilder app)
        {
            
            var container = app.ApplicationServices.GetService(typeof(IUnityContainer)) as IUnityContainer;

            if (container == null)
            {
                throw new Exception("You need to configure your unity container first.");
            }

            var essence = container.GetEssence();

            return essence;
        }
        
        public static IApplicationBuilder UseFixtureWithUnity<TFixture>(this IApplicationBuilder app)
        {
            var essence = GetEssence(app);
            
            var executer = new FixtureExecuter(essence);

            try
            {
                executer.Execute<TFixture>();
            }
            catch (Exception e)
            {
                essence.Logger.LogError(e,"There was a problem executing your fixture: {Exception}",e);
            }

            return app;
        }
    }
}