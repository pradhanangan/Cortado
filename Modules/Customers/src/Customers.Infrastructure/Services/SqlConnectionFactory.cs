using Customers.Application.Common.Interfaces;
using Npgsql;
using System.Data.Common;

namespace Customers.Infrastructure.Services;

public class SqlConnectionFactory(NpgsqlDataSource dataSource) : ISqlConnectionFactory
{
    public async ValueTask<DbConnection> OpenConnectionAsync()
    {
        return await dataSource.OpenConnectionAsync();
    }
}
