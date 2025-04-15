using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;

namespace Products.Application.Products;

public record GetProductsByCustomerIdQuery(Guid CustomerId) : IRequest<List<ProductDto>>;


public class GetProductsByCustomerIdQueryHandler(IProductDbContext dbContext) : IRequestHandler<GetProductsByCustomerIdQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .AsNoTracking()
            .Where(b => b.CustomerId == request.CustomerId).ToListAsync();
        
        return products.Adapt<List<ProductDto>>();
    }
}
