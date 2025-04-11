using Customers.Application.Common.Interfaces;
using Customers.Domain.Common;
using Customers.Domain.Entities;
using MediatR;

namespace Customers.Application.Customers;

public sealed record CreateCustomerCommand(string Username, string Email, Guid IdentityId) : IRequest<Result<Guid>>;

public class CreateCustomerCommandHandler(ICustomersDbContext dbContext) : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            IdentityId = request.IdentityId,
        };
        dbContext.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}

