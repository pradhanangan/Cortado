using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public record SearchProductsByCodeQuery(string Code) : IRequest<Result<List<ProductDto>>>;

public class SearchProductsByCodeQueryHandler(IServiceScopeFactory serviceScopeFactory) : IRequestHandler<SearchProductsByCodeQuery, Result<List<ProductDto>>>
{
    public async Task<Result<List<ProductDto>>> Handle(SearchProductsByCodeQuery request, CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IProductDbContext>();
        
        var products = await dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .Where(p => p.Code.ToLower().Contains(request.Code.ToLower()))
            .ToListAsync(cancellationToken);

        return products.Adapt<List<ProductDto>>();
    }
}

