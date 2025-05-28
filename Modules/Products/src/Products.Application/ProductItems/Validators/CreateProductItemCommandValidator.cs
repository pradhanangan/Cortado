using FluentValidation;

namespace Products.Application.ProductItems.Validators;

public class CreateProductItemCommandValidator : AbstractValidator<CreateProductItemCommand>
{
    public CreateProductItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Variants).NotEmpty();
       
        RuleFor(x => x.UnitPrice).NotEmpty().When(x => !x.IsFree);
    }
}