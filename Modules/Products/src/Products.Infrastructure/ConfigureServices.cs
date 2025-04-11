using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Interfaces;
using Products.Infrastructure.Persistance;

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

        //services.AddScoped<IProductDbContext, ProductDbContext>();
        services.AddScoped<IProductDbContext>(provider => provider.GetService<ProductDbContext>());
        return services;
    }
}
