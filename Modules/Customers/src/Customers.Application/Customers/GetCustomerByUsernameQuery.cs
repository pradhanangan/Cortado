using Customers.Application.Common.Exceptions;
using Customers.Application.Common.Interfaces;
using Customers.Application.Customers.Dtos;
using Customers.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Customers.Application.Customers;

public sealed record GetCustomerByUsernameQuery(string Username) : IRequest<Result<CustomerDto>>;

public class GetCustomerByUsernameQueryHandler(ICustomersDbContext dbContext) : IRequestHandler<GetCustomerByUsernameQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByUsernameQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Username.ToLower() == request.Username.ToLower(), cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException(nameof(Customer), request.Username);
        }
        return customer.Adapt<CustomerDto>();
    }
}
