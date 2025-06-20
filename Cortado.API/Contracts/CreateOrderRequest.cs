﻿namespace Cortado.API.Contracts;

public sealed record CreateOrderRequest(Guid ProductId, string Email, string PhoneNumber, string FirstName, string LastName, DateTime OrderDate, List<OrderItem> OrderItems);
public sealed record CreateOrderWithPaymentRequest(Guid ProductId, string Email, string PhoneNumber, string FirstName, string LastName, DateTime OrderDate, List<OrderItem> OrderItems, string PaymentId);
public sealed record OrderItem(Guid ProductItemId, int Quantity);