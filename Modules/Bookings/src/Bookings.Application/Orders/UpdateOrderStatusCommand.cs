using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public sealed record UpdateOrderStatusCommand(Guid OrderId, string PaymentId, bool IsPaid, bool IsConfirmed) : IRequest<Result<Unit>>;

public class UpdateOrderStatusCommandHandler(IBookingsDbContext bookingsDbContext) : IRequestHandler<UpdateOrderStatusCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>().FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        order.IsPaid = request.IsPaid;
        order.IsConfirmed = request.IsConfirmed;
        order.PaymentId = request.PaymentId;

        await bookingsDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}