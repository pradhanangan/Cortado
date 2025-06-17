namespace Bookings.Application.Common.Exceptions;

public class TokenExpiredException : TokenValidationException
{
    public TokenExpiredException()
        : base("Token has expired.")
    {
    }
}
