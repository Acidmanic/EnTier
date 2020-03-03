


using AutoMapper;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;

public static class RegisterServices
{
    

    public static void AddApplicationServices(this IServiceCollection services){


        services.AddScoped<ILoginService,DefaultLoginService>();
        
        services.AddSingleton<IMapper>(sp => AutomapperBootstrap.CreateMapper());

        services.AddScoped<IProvider<DatabaseUnit>, DatabaseUnitProvider>();
    }
}