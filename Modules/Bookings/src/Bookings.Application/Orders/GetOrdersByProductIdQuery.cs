using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Application.Orders;


public sealed record GetOrdersByProductIdQuery(Guid ProductId) : IRequest<List<OrderDto>>;

public class GetOrdersByProductIdQueryHandler(IBookingsDbContext bookingDbContext) : IRequestHandler<GetOrdersByProductIdQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetOrdersByProductIdQuery request, CancellationToken cancellationToken)
    {
        return await bookingDbContext.Set<Order>()
            .Where(x => x.ProductId == request.ProductId).ProjectToType<OrderDto>()
            .ToListAsync(cancellationToken);
    }
}
