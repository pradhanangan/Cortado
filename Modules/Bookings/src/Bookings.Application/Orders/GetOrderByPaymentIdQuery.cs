using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Products;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public record GetOrderByPaymentIdQuery(string PaymentId) : IRequest<Result<OrderDto>>;


public class GetOrderByPaymentIdQueryHandler(IBookingsDbContext bookingsDbContext, ISender sender) : IRequestHandler<GetOrderByPaymentIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByPaymentIdQuery request, CancellationToken cancellationToken)
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
        foreach (var oi in order.OrderItems)
        {
            var productItemName = product.ProductItems.FirstOrDefault(pi => pi.Id == oi.ProductItemId)?.Name ?? "";
            orderItemDtos.Add(new OrderItemDto(oi.Id, oi.OrderId, oi.ProductItemId, productItemName, oi.UnitPrice, oi.Quantity, oi.LineTotal, new()));
        }

        var orderDto = new OrderDto(order.Id, order.OrderNumber, order.ProductId, order.Email, order.PhoneNumber, order.OrderDate, orderItemDtos, order.SubTotal, order.TotalAmount, order.IsVerified, order.IsPaid, order.PaymentId, order.IsConfirmed);

        return orderDto;
    }
}
