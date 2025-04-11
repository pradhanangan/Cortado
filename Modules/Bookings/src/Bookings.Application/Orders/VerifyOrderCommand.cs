using Bookings.Application.Common.Constants;
using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;

namespace Bookings.Application.Orders;

public sealed record class VerifyOrderCommand(string Token) : IRequest<VerifyOrderResponse>;
public sealed record class VerifyOrderResponse(string Status);

public class VerifyOrderCommandHandler(IBookingsDbContext bookingDbContext, ITokenService tokenService) : IRequestHandler<VerifyOrderCommand, VerifyOrderResponse>
{
    public async Task<VerifyOrderResponse> Handle(VerifyOrderCommand request, CancellationToken cancellationToken)
    {
        (string orderId, string email) = ("", "");
        try
        {
            (orderId, email) = tokenService.ValidateBookingVerificationToken(request.Token);
        }
        catch (Exception ex)
        //when (ex is TokenExpiredException)
        {
            throw;
            //return new VerifyBookingResponse(VerifyBookingStatus.TokenExpired);
        }

        var order = bookingDbContext.Set<Order>().Single(b => b.Id == Guid.Parse(orderId));

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        if (order.IsVerified == true)
        {
            //throw new BadRequestException("Email already verified");
            return new VerifyOrderResponse(VerifyBookingStatus.AlreadyVerified);
        }

       order.IsVerified = true;
        await bookingDbContext.SaveChangesAsync(cancellationToken);

        return new VerifyOrderResponse(VerifyBookingStatus.Verified);
    }
}