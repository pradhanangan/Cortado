namespace Products.Application.Common.Exceptions;

public class MissingClaimException: TokenValidationException
{
    public MissingClaimException(string ClaimType) 
        : base($"Token is missing the required claim `{ClaimType}`")
    {
        
    }
}
