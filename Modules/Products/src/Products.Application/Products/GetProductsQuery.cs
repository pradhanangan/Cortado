using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public record GetProductsQuery(Guid CustomerId) : IRequest<Result<List<ProductDto>>>;

public class GetProductsQueryHandler(IServiceScopeFactory serviceScopeFactory) : IRequestHandler<GetProductsQuery, Result<List<ProductDto>>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IProductDbContext>();
        if (dbContext == null) {
            throw new Exception("dbcontext is null");
        }
        
        var products = dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .AsNoTracking()
            .Where(b => b.CustomerId == request.CustomerId);
        return await products.ProjectToType<ProductDto>().ToListAsync();
    }
}

