using EnTier;
using EnTier.Fixture;

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
    }
}