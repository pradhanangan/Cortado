using Customers.Application.Common.Exceptions;
using Customers.Application.Common.Interfaces;
using Customers.Application.Customers.Dtos;
using Customers.Domain.Common;
using Customers.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Customers.Application.Customers;

public sealed record GetCustomerByEmailQuery(string Email) : IRequest<Result<CustomerDto>>;

public class GetCustomerByEmailQueryHandler(ICustomersDbContext dbContext) : IRequestHandler<GetCustomerByEmailQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Username.ToLower() == request.Email.ToLower(), cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException(nameof(Customer), request.Email);
        }
        return customer.Adapt<CustomerDto>();
    }
}