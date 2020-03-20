



using Configuration;
using DIBinding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Utility;

public static class MSDIInjectorExtensios{




    public static IServiceCollection AddEnTierServices(this IServiceCollection services){

        EnTierApplication.Initialize(services);

        return services;
    }


    public static IApplicationBuilder UseEnTier(this IApplicationBuilder app){

        EnTierApplication.Initialize(app);
        
        return app;
    }


}