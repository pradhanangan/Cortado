using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;

namespace Products.Application.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler(IProductDbContext dbContext) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>()
            .Include(p => p.ProductItems)
            .FirstOrDefaultAsync(b => b.Id == request.Id);
        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }
        return product.Adapt<ProductDto>();
    }
}
