using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using Bookings.Infrastructure.Persistance;
using Bookings.Infrastructure.Persistance.Repositories;
using Bookings.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Bookings.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddBookingsInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
        
        services.AddDbContext<BookingsDbContext>(options => 
        {
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddScoped<ISqlConnectionFactory, SqlConnectionFactory>();


        services.AddScoped<IBookingsDbContext, BookingsDbContext>();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IQrCodeService, SimpleQrCodeService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IStripePaymentService, StripePaymentService>();
        return services;
    }
}
