using Bookings.Application.Common.Interfaces;
using Bookings.Application.Orders.Dtos;
using Bookings.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Interfaces;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;


public sealed record GetOrdersByProductCodeQuery(string ProductCode) : IRequest<Result<List<OrderDto>>>;

public class GetOrdersByProductCodeQueryHandler(IProductDbContext productDbContext, IBookingsDbContext bookingDbContext) : IRequestHandler<GetOrdersByProductCodeQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(GetOrdersByProductCodeQuery request, CancellationToken cancellationToken)
    {
        var product = await productDbContext.Set<Product>().FirstOrDefaultAsync(x => x.Code == request.ProductCode);
        if (product is null)
        {
            return Result.Failure<List<OrderDto>>(new Error("Error", "Product not found"));
        }

        return await bookingDbContext.Set<Order>()
            .Where(x => x.ProductId == product.Id).ProjectToType<OrderDto>()
            .ToListAsync(cancellationToken);
    }
}
