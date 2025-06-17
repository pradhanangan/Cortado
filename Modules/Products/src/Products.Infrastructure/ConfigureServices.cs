using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Interfaces;
using Products.Infrastructure.Persistance;
using Products.Infrastructure.Services;

namespace Products.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddProductsInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });
        
        services.AddScoped<IProductDbContext>(provider => provider.GetService<ProductDbContext>());

        services.AddScoped<IProductTokenService, ProductTokenService>();
        return services;
    }
}
