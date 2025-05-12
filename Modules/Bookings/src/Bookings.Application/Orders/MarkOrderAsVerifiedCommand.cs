using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using Bookings.Domain.Errors;
using MediatR;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public sealed record MarkOrderAsVerifiedCommand(Guid OrderId):IRequest<Result<bool>>;


public class MarkOrderAsVerifiedCommandHandler(IBookingsDbContext bookingsDbContext) : IRequestHandler<MarkOrderAsVerifiedCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkOrderAsVerifiedCommand request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>().FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order == null)
        {
            return Result.Failure<bool>(OrderErrors.OrderNotFound);
        }

        order.IsVerified = true;
        await bookingsDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}