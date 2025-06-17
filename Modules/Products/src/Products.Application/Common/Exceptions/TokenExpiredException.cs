namespace Products.Application.Common.Exceptions;

public class TokenExpiredException : TokenValidationException
{
    public TokenExpiredException(string ClaimType)
    : base($"Token is missing the required claim `{ClaimType}`")
    {
    }
}
