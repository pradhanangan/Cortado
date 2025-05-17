using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Products.Application.Common.Configurations;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public record GetProductsByCustomerIdQuery(Guid CustomerId) : IRequest<Result<List<ProductDto>>>;

public class GetProductsByCustomerIdQueryHandler(IProductDbContext dbContext, IOptions<WebClientSettings> webClientSettingsOptions) : IRequestHandler<GetProductsByCustomerIdQuery, Result<List<ProductDto>>>
{
    private readonly WebClientSettings _webClientSettings = webClientSettingsOptions.Value;

    public async Task<Result<List<ProductDto>>> Handle(GetProductsByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .AsNoTracking()
            .Where(b => b.CustomerId == request.CustomerId).ToListAsync();

        return products.Adapt<List<ProductDto>>();
            
    }
}