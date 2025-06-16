using FluentValidation;

namespace Products.Application.Products.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty();
        RuleFor(x => x.EndDate.ToDateTime(x.EndTime))
            .GreaterThan(x => x.StartDate.ToDateTime(x.StartTime));
    }
}
