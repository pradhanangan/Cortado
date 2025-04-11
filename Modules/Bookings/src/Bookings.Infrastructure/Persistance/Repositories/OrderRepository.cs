using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using Dapper;

namespace Bookings.Infrastructure.Persistance.Repositories;

public class OrderRepository(IBookingsDbContext bookingDbContext, ISqlConnectionFactory sqlConnectionFactory) : IOrderRepository
{
    public async Task<int> GetNextOrderNumberAsync()
    {

        var sql = "SELECT nextval('order_number_seq')";

        using var connection = await sqlConnectionFactory.OpenConnectionAsync();
        return await connection.QuerySingleAsync<int>(sql);
    }
}
