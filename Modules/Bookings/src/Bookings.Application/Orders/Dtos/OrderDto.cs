namespace Bookings.Application.Orders.Dtos;

public sealed record OrderDto(Guid Id, string OrderNumber, Guid ProductId, string Email, string PhoneNumber, DateTime OrderDate, List<OrderItemDto> OrderItems, decimal SubTotal, decimal TotalAmount, bool IsVerified, bool IsPaid, string PaymentId, bool IsConfirmed)
{
    public int TotalNumberOfTickets => OrderItems?.Sum(item => item.Quantity) ?? 0;
}
