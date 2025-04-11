using System.Data.Common;

namespace Bookings.Application.Common.Interfaces;

public interface ISqlConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}
