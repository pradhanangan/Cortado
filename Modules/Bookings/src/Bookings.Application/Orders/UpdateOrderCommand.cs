using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace Bookings.Application.Orders;

public sealed record UpdateOrderCommand(Guid Id, Guid ProductId, string Email, string PhoneNumber, bool IsEmailVerified, bool IsPaid, bool IsConfirmed, DateTime OrderDate, List<CreateOrderItem> OrderItems, string PaymentId) : IRequest<Guid>;

public class UpdateOrderCommandHandler(IBookingsDbContext bookingDbContext, IMapper mapper) : IRequestHandler<UpdateOrderCommand, Guid>
{
    public async Task<Guid> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
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
