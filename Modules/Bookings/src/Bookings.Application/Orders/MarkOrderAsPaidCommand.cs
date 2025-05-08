using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public sealed record MarkOrderAsPaidCommand(Guid OrderId, string PaymentId): IRequest<Result<bool>>;

public class MarkOrderAsPaidCommandHandler(IBookingsDbContext bookingsDbContext) : IRequestHandler<MarkOrderAsPaidCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkOrderAsPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await bookingsDbContext.Set<Order>().FirstOrDefaultAsync(o => o.Id == request.OrderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        order.IsPaid = true;
        order.PaymentId = request.PaymentId; 

        await bookingsDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}