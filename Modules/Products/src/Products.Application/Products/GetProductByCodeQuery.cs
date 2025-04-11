using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;

namespace Products.Application.Products;

public record GetProductByCodeQuery(string Code) : IRequest<ProductDto>;

public class GetProductByCodeQueryHandler(IProductDbContext dbContext) : IRequestHandler<GetProductByCodeQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByCodeQuery request, CancellationToken cancellationToken)
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

