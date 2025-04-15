using MediatR;
using Products.Application.Common.Interfaces;
using Products.Domain.Entities;

namespace Products.Application.Products;

public sealed record CreateProductCommand(Guid CustomerId, string Code, string Name, string Description, DateOnly StartDate, DateOnly EndDate) : IRequest<Guid>;

public class CreateProductCommandHandler(IProductDbContext dbContext) : IRequestHandler<CreateProductCommand, Guid>
{
    
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Code = request.Code, 
            Name = request.Name, 
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        dbContext.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return product.Id;
    }
}

