using Bookings.Application.Tickets.Dtos;

namespace Bookings.Application.Orders.Dtos;

public sealed record OrderItemDto(Guid Id, Guid OrderId, Guid ProductItemId, string ProductItemName, decimal UnitPrice, int Quantity, decimal LineTotal, List<TicketDto> Tickets);
