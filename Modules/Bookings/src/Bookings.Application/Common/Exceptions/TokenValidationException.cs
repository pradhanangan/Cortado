namespace Bookings.Application.Common.Exceptions;

public class TokenValidationException(string message) : Exception(message)
{
}
