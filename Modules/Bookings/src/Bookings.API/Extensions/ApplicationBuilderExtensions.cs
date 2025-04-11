using Bookings.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bookings.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
