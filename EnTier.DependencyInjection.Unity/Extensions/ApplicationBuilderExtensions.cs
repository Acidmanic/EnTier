using System;
using EnTier;
using EnTier.Exceptions;
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
                throw new EnTierEssenceIsNotRegisteredException();
            }

            var essence = container.GetEssence();

            return essence;
        }
      
    }
}