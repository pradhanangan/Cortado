namespace Cortado.API.Contracts;

public sealed record UpdateOrderStatusRequest(Guid OrderId, string PaymentId, bool IsPaid, bool IsConfirmed);