


using AutoMapper;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class RegisterServices
{
    

    public static void AddApplicationServices(this IServiceCollection services){


        services.AddScoped<ILoginService,DefaultLoginService>();
        
        services.AddSingleton<IMapper>(sp => AutomapperBootstrap.CreateMapper());

        services.AddScoped<IProvider<DatabaseUnit>, DatabaseUnitProvider>();

        services.AddScoped<IUsersService,UsersService>();
    }
}