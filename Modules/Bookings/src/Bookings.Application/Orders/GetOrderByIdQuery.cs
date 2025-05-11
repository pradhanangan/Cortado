using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Application.Tickets.Dtos;
using Bookings.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Products;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;

public class GetOrderByIdQueryHandler(IBookingsDbContext bookingsDbContext, ISender sender) : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Tickets)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        var productResult = await sender.Send(new GetProductByIdQuery(order.ProductId));
        if (productResult.IsFailure)
        {
            throw new NotFoundException("Product", order.ProductId);
        }

        var product = productResult.Value;
        var itemLookup = product.ProductItems.ToDictionary(i => i.Id, i => i.Name);

        var orderItemDtos = new List<OrderItemDto>();
        
        
        foreach (var oi in order.OrderItems)
        {
            var productItemName = product.ProductItems.FirstOrDefault(pi => pi.Id == oi.ProductItemId)?.Name ?? "";
            var ticketDtos = oi.Tickets.Select(t => new TicketDto(t.TicketNumber, t.IsUsed, t.UsedDate)).ToList();

            orderItemDtos.Add(new OrderItemDto(oi.Id, oi.OrderId, oi.ProductItemId, productItemName, oi.UnitPrice, oi.Quantity, oi.LineTotal, ticketDtos));    
        }
        
        var orderDto = new OrderDto(order.Id, order.OrderNumber, order.ProductId, order.Email, order.PhoneNumber, order.OrderDate, orderItemDtos, order.SubTotal, order.TotalAmount, order.IsVerified, order.IsPaid, order.PaymentId, order.IsConfirmed);
        
        return orderDto;
    }
}
