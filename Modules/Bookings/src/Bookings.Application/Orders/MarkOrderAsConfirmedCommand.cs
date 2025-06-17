using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using Bookings.Domain.Errors;
using MediatR;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public sealed record class MarkOrderAsConfirmedCommand(Guid OrderId) : IRequest<Result<bool>>;

public class MarkOrderAsConfirmedHandler(IBookingsDbContext bookingsDbContext) : IRequestHandler<MarkOrderAsConfirmedCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkOrderAsConfirmedCommand request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>().FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order == null)
        {
            return Result.Failure<bool>(OrderErrors.OrderNotFound);
        }
        
        if(!order.IsVerified)
        {
            return Result.Failure<bool>(OrderErrors.OrderNotVerified);
        }

        if (!order.IsPaid)
        {
            return Result.Failure<bool>(OrderErrors.OrderNotPaid);
        }

        if(string.IsNullOrEmpty(order.PaymentId))
        {
            return Result.Failure<bool>(OrderErrors.InvalidPayment);
        }

        order.IsConfirmed = true;
        await bookingsDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

