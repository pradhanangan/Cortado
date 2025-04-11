using Bookings.Application.Common.Interfaces;
using Npgsql;
using System.Data.Common;

namespace Bookings.Infrastructure.Services;

public class SqlConnectionFactory(NpgsqlDataSource dataSource) : ISqlConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        return await dataSource.OpenConnectionAsync();
    }
}
