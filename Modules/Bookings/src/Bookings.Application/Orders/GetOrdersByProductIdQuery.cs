using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;


public sealed record GetOrdersByProductIdQuery(Guid ProductId) : IRequest<Result<List<OrderDto>>>;

public class GetOrdersByProductIdQueryHandler(IBookingsDbContext bookingDbContext) : IRequestHandler<GetOrdersByProductIdQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(GetOrdersByProductIdQuery request, CancellationToken cancellationToken)
    {
        return await bookingDbContext.Set<Order>()
            .Where(x => x.ProductId == request.ProductId).ProjectToType<OrderDto>()
            .ToListAsync(cancellationToken);
    }
}
