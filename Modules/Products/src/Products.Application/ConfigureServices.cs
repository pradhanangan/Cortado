using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Configurations;
using System.Reflection;

namespace Products.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddProductsApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure settings
        services.Configure<ProductsSettings>(configuration.GetSection("ProductsSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<WebClientSettings>(configuration.GetSection("WebClient"));

        // Register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
