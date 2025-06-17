using Shared.Common.Abstraction;

namespace Bookings.Domain.Errors;

public static class OrderErrors
{
    public static Error OrderNotFound = new("Order.NotFound", "Order not found");
    public static Error OrderNotVerified = new("Order.NotVerified", "Order not verified");
    public static Error OrderNotPaid = new("Order.NotPaid", "Order not paid");
    public static Error InvalidPayment = new("Order.InvalidPayment", "Invalid payment");
}
