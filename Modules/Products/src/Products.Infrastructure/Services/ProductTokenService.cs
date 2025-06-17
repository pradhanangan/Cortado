using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Products.Application.Common.Configurations;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Products.Infrastructure.Services;

public class ProductTokenService (IOptions<JwtSettings> jwtSetttingsOptions): IProductTokenService
{
    private readonly JwtSettings _jwtSettings = jwtSetttingsOptions.Value;

    public string GenerateProductToken(Guid productId, DateTime expiry)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim("ProductId", productId.ToString()) };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid ValidateProductVerificationToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = key,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            var productId = principal.FindFirst("ProductId")?.Value;
            
            if (string.IsNullOrEmpty(productId))
            {
                throw new MissingClaimException("ProductId");
            }

            return Guid.Parse(productId);

        }
        catch (SecurityTokenExpiredException ex)
        {
            throw new TokenExpiredException("Token has expired");
        }
        catch (Exception ex)
        {
            // Log the exception
            throw;
        }
    }
}
