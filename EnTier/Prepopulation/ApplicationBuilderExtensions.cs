using System.Reflection;
using EnTier.Prepopulation;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensionsForPrepopulation
    {
  
        public static void PerformPrepopulation(this IApplicationBuilder app,Assembly assembly)
        {
            var resolver = new DotnetResolver(app);

            PrepopulationManager2.GetInstance().PerformPrepopulation(resolver,assembly);
        }
        
        
    }
}