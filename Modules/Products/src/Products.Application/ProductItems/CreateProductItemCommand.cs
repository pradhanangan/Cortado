using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using Products.Domain.Entities;

namespace Products.Application.ProductItems;

public sealed record CreateProductItemCommand(Guid ProductId, string Name, string Description, string Variants, decimal UnitPrice) : IRequest<Guid>;

public class CreateProductItemCommandHandler(IProductDbContext dbContext) : IRequestHandler<CreateProductItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductItemCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Set<Product>().SingleOrDefaultAsync(p => p.Id == request.ProductId);
        if(product is null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        var productItem = new ProductItem
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Name = request.Name,
            Description = request.Description,
            Variants = request.Variants,
            UnitPrice = request.UnitPrice
        };

        dbContext.Add(productItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return productItem.Id;
    }
}

