using System.Data.Common;

namespace Customers.Application.Common.Interfaces;

public interface ISqlConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
