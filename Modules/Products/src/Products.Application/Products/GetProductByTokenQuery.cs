using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public sealed record GetProductByTokenQuery(string token) : IRequest<Result<ProductDto>>;

public class GetProductByTokenQueryHandler(IProductDbContext dbContext, IProductTokenService productTokenService) : IRequestHandler<GetProductByTokenQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByTokenQuery request, CancellationToken cancellationToken)
    {
        Guid? productId = null;
        try
        {
            productId = productTokenService.ValidateProductVerificationToken(request.token);
        }
        catch (Exception ex)
        {
            var error = ex switch
            {
                TokenExpiredException => new Error("Token.TokenExpired", "The provided token has expired."),
                MissingClaimException => new Error("Token.MissingClaim", "The token is missing the required claim `ProductId`"),
                _ => new Error("Token.UnknownError", "An unknown error occurred while validating the token."),
            };

            return Result.Failure<ProductDto>(error);
        }

        var product = await dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
        {
            return Result.Failure<ProductDto>(new Error("Product.NotFound", "Product not found"));
        }
        return product.Adapt<ProductDto>();
    }
}