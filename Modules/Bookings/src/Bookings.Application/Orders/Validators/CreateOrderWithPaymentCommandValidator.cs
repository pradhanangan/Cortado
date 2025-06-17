using FluentValidation;

namespace Bookings.Application.Orders.Validators;

public class CreateOrderWithPaymentCommandValidator : AbstractValidator<CreateOrderWithPaymentCommand>
{
    public CreateOrderWithPaymentCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(40);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(40);
        RuleFor(x => x.OrderItems)
            .NotEmpty();
        RuleFor(x => x.OrderDate)
            .NotEmpty();
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .MaximumLength(20);
    }
}