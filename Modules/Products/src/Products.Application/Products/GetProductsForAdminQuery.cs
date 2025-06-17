using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public record GetProductsForAdminQuery() : IRequest<Result<List<ProductDto>>>;

public class GetProductsForAdminQueryHandler(IProductDbContext dbContext) : IRequestHandler<GetProductsForAdminQuery, Result<List<ProductDto>>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductsForAdminQuery request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .AsNoTracking()
            .ToListAsync();

        return products.Adapt<List<ProductDto>>();
    }
}