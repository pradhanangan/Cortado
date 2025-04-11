using Customers.Application.Common.Interfaces;
using Customers.Infrastructure.Persistance;
using Customers.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Customers.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddCustomersInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<CustomersDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddScoped<ISqlConnectionFactory, SqlConnectionFactory>();


        services.AddScoped<ICustomersDbContext, CustomersDbContext>();
        
        return services;
    }
}
