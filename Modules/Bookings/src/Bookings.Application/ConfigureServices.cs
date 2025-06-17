using Bookings.Application.Configurations;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bookings.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddBookingsApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Register MediatR
        //services.AddMediatR(cfg => Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

 

        // Register JwtSettings from appsettings.json in API layer
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<WebClientSettings>(configuration.GetSection("WebClientSettings"));
        return services;
    }
}
