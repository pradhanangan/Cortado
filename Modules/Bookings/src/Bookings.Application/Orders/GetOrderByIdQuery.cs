using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Interfaces;
using Products.Application.Products;

namespace Bookings.Application.Orders;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;

public class GetOrderByIdQueryHandler(IBookingsDbContext bookingsDbContext, ISender sender) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Tickets)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

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
