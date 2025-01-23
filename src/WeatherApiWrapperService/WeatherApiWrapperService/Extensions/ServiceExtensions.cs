using Microsoft.OpenApi.Models;
using StackExchange.Redis;
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

    public static void ConfigureRedis(this WebApplicationBuilder builder)
    {
        // Install-Package Microsoft.Extensions.Caching.StackExchangeRedis


        // Adding Redis service
        // It adds and configures Redis distributed cache service 
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            //This property is set to specify the connection string for Redis
            //The value is fetched from the application's configuration system, i.e., appsettings.json file
            options.Configuration = builder.Configuration["RedisCacheOptions:Configuration"];
            //This property helps in setting a logical name for the Redis cache instance. 
            //The value is also fetched from the appsettings.json file
            options.InstanceName = builder.Configuration["RedisCacheOptions:InstanceName"];
        });


        // Register the Redis connection multiplexer as a singleton service
        // This allows the application to interact directly with Redis for advanced scenarios
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            // Establish a connection to the Redis server using the configuration from appsettings.json
            ConnectionMultiplexer.Connect(builder.Configuration["RedisCacheOptions:Configuration"]));
    }
}
