using Shared.Common.Abstraction;

namespace Products.Domain.Errors;

public static class ProductErrors
{
    public static Error ProductTokenExpired = new("Product.TokenExpired", "Token expired.");
}
