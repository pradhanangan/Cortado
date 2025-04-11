namespace Cortado.API.Contracts;

public sealed record UpdateOrderRequest(Guid Id,
    Guid ProductId,
    string Email,
    string PhoneNumber,
    bool IsEmailVerified,
    bool IsPaid,
    bool IsConfirmed,
    DateTime OrderDate,
    List<OrderItem> OrderItems,
    decimal TotalPrice,
    string PaymentId);
