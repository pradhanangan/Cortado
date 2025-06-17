using Bookings.Application.Common.Constants;
using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public sealed record class VerifyOrderCommand(string Token) : IRequest<Result<VerifyOrderResponse>>;
public sealed record class VerifyOrderResponse(string Status);

public class VerifyOrderCommandHandler(IBookingsDbContext bookingDbContext, ITokenService tokenService) : IRequestHandler<VerifyOrderCommand, Result<VerifyOrderResponse>>
{
    public async Task<Result<VerifyOrderResponse>> Handle(VerifyOrderCommand request, CancellationToken cancellationToken)
    {
        (string orderId, string email) = ("", "");
        try
        {
            (orderId, email) = tokenService.ValidateBookingVerificationToken(request.Token);
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case TokenExpiredException:
                    return new VerifyOrderResponse(VerifyBookingStatus.TokenExpired);
                case MissingClaimException:
                    return Result.Failure<VerifyOrderResponse>(new Error("Token.MissingClaim", "The token is missing the required claim `BookingId or Email`"));
                default:
                    return Result.Failure<VerifyOrderResponse>(new Error("Token.UnknownError", "An unknown error occurred while validating the token."));
            }
        }

        var order = bookingDbContext.Set<Order>().Single(b => b.Id == Guid.Parse(orderId));

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), orderId);
        }

        if (order.IsVerified == true)
        {
            return new VerifyOrderResponse(VerifyBookingStatus.AlreadyVerified);
        }

        order.IsVerified = true;
        await bookingDbContext.SaveChangesAsync(cancellationToken);

        return new VerifyOrderResponse(VerifyBookingStatus.Verified);
    }
}