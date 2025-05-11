namespace Bookings.Application.Tickets.Dtos;

public sealed record TicketDto(string TicketNumber, bool IsUsed, DateTime? UsedDate);

