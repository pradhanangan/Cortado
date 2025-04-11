using System.IdentityModel.Tokens.Jwt;

namespace Shared.Common.Authentication;

public class TokenParser : ITokenParser
{
    public string? GetSubFromIdToken(string idToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(idToken);

        return jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
    }

    public string? GetEmailFromIdToken(string idToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(idToken);

        return jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
    }
}
