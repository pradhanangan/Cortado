using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MapsterMapper;
using MediatR;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public sealed record UpdateOrderCommand(Guid Id, Guid ProductId, string Email, string PhoneNumber, bool IsVerified, bool IsPaid, bool IsConfirmed, DateTime OrderDate, string PaymentId) : IRequest<Result<Guid>>;

public class UpdateOrderCommandHandler(IBookingsDbContext bookingDbContext, IMapper mapper) : IRequestHandler<UpdateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await bookingDbContext.Set<Order>().FindAsync(request.Id);
        if (order is null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        mapper.Map(request, order);

        await bookingDbContext.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
