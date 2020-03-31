
using System;
using System.Diagnostics;
using System.Reflection;
using EnTier.Configuration;
using EnTier.Binding;
using Microsoft.AspNetCore.Builder;
using EnTier.Utility;
using EnTier;
using EnTier.Bootstrap;

namespace Microsoft.Extensions.DependencyInjection
{


    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddEnTierServices(this IServiceCollection services)
        {

            Boot.Strap(services);

            return services;
        }


    }
}



