using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Common.Exceptions;
using Products.Application.Common.Interfaces;
using Products.Domain.Entities;
using Shared.Common.Abstraction;
using Shared.Common.Exceptions;

namespace Products.Application.ProductItems;

public sealed record CreateProductItemCommand(Guid ProductId, string Name, string Description, string Variants, bool IsFree, decimal UnitPrice) : IRequest<Result<Guid>>;

public class CreateProductItemCommandHandler(IProductDbContext dbContext, IValidator<CreateProductItemCommand> validator) : IRequestHandler<CreateProductItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductItemCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new RequestValidationException(validationResult.Errors);
        }

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
            IsFree = request.IsFree,
            UnitPrice = request.UnitPrice
        };

        dbContext.Add(productItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return productItem.Id;
    }
}

