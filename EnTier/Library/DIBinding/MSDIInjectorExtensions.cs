



using Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utility;

public static class MSDIInjectorExtensios{




    public static IServiceCollection AddEnTierServices(this IServiceCollection services){

        //TODO: Later figure a way to give the code user the chance to choos what 
        // configuration provider then want to use.
        services.AddSingleton<IProvider<EnTierConfigurations>,MSExtensionsConfigurationsProvider>();

        return services;
    }


}