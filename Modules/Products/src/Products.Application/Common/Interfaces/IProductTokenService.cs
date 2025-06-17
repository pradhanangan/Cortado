namespace Products.Application.Common.Interfaces;

public interface IProductTokenService
{
    string GenerateProductToken(Guid productId, DateTime expiry);
    Guid ValidateProductVerificationToken(string token);
}
