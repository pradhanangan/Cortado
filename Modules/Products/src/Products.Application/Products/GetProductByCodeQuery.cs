using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public record GetProductByCodeQuery(string Code) : IRequest<Result<ProductDto>>;

public class GetProductByCodeQueryHandler(IProductDbContext dbContext) : IRequestHandler<GetProductByCodeQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByCodeQuery request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>()
           .Include(p => p.ProductItems)
           .FirstOrDefaultAsync(p => p.Code.ToLower() ==  request.Code.ToLower());

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.Code);
        }
        return product.Adapt<ProductDto>();
    }
}

