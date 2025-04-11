namespace Bookings.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateBookingVerificationToken(string bookingId, string email);
    (string, string) ValidateBookingVerificationToken(string token);
}
