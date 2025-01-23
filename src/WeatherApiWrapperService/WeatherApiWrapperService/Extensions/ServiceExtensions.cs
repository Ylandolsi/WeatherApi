using Microsoft.OpenApi.Models;
using WeatherApiWrapperService.Contracts;
using WeatherApiWrapperService.Services;

namespace WeatherApiWrapperService.Extensions;
public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );
        });
    


    public static void ConfigureSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "toDoList api ", Version = "v1" });
        });
    }

    public static void ConfigureWeatherService(this IServiceCollection services)
    {
        services.AddScoped<IWeatherService, WeatherService>(); 
    }
    

}
