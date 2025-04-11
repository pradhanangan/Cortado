using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using Dapper;

namespace Bookings.Infrastructure.Persistance.Repositories;

public class TicketRepository(IBookingsDbContext applicationDbContext, ISqlConnectionFactory sqlConnectionFactory) : ITicketRepository
{
    public async Task<int> GetNextTicketNumberAsync()
    {

        var sql = "SELECT nextval('ticket_number_seq')";

        //await using var command = applicationDbContext.ChangeTracker.Context.Database.GetDbConnection().CreateCommand();
        //command.CommandText = sql;
        //await applicationDbContext.ChangeTracker.Context.Database.OpenConnectionAsync();
        //var result = await command.ExecuteScalarAsync();
        //return Convert.ToInt32(result);


        //var a = applicationDbContext.ChangeTracker.Context.Database.SqlQuery<int>(FormattableStringFactory.Create(sql));
        //return a.FirstOrDefault();

        //var sql = "SELECT nextval('ticket_number_seq') AS Value";
        //var result = await applicationDbContext.Set<ScalarResult<int>>()
        //    .FromSqlRaw(sql)
        //    .AsNoTracking()
        //    .FirstOrDefaultAsync();

        //return result?.Value ?? 0;
        using var connection = await sqlConnectionFactory.OpenConnectionAsync();
        return await connection.QuerySingleAsync<int>(sql);
    }

    //private class ScalarResult<T>
    //{
    //    public T Value { get; set; }
    //}
}
