using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Products;

namespace Bookings.Application.Orders;

public record GetOrderByPaymentIdQuery(string PaymentId) : IRequest<OrderDto>;


public class GetOrderByPaymentIdQueryHandler(IBookingsDbContext bookingsDbContext, ISender sender) : IRequestHandler<GetOrderByPaymentIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByPaymentIdQuery request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Tickets)
            .FirstOrDefaultAsync(x => x.PaymentId == request.PaymentId);

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), request.PaymentId);
        }
        
        var productResult = await sender.Send(new GetProductByIdQuery(order.ProductId));
        if (productResult.IsFailure)
        {
            throw new NotFoundException("Product", order.ProductId);
        }

        var product = productResult.Value;

        var orderItemDtos = new List<OrderItemDto>();
        decimal totalPrice = 0;
        foreach (var oi in order.OrderItems)
        {
            var productItemName = product.ProductItems.FirstOrDefault(pi => pi.Id == oi.ProductItemId)?.Name ?? "";
            orderItemDtos.Add(new OrderItemDto(oi.Id, oi.OrderId, oi.ProductItemId, productItemName, oi.Quantity, oi.Price, new()));
            totalPrice += oi.Price;
        }

        var orderDto = new OrderDto(order.Id, order.ProductId, order.Email, order.PhoneNumber, order.IsVerified, order.IsPaid, order.IsConfirmed, order.OrderDate, totalPrice, orderItemDtos, order.PaymentId);

        return orderDto;
    }
}
