namespace Bookings.Application.Common.Exceptions;

public class TokenExpiredException : Exception
{
    public TokenExpiredException()
        : base("Token has expired.")
    {
    }
}
