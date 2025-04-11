namespace Bookings.Domain.Entities;

public interface IOrderRepository
{
    Task<int> GetNextOrderNumberAsync();
}
