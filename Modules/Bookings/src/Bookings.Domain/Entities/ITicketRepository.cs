namespace Bookings.Domain.Entities;

public interface ITicketRepository
{
    Task<int> GetNextTicketNumberAsync();
}
