namespace Bookings.Application.Orders.Dtos;

public sealed record OrderDto(Guid Id, Guid ProductId, string Email, string PhoneNumber, bool IsEmailVerified, bool IsPaid, bool IsConfirmed, DateTime OrderDate, decimal TotalPrice, List<OrderItemDto> OrderItems, string PaymentId)
{
    public int TotalNumberOfTickets => OrderItems.Sum(item => item.Quantity);
}
